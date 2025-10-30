using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public abstract class EffectActionTarget : IDescribable
{
    public abstract string Description { get; }

    public CharacterTargets GetTargetsFrom(CharacterIdentifier user)
        => new CharacterTargetsFromUser(this, user);
    public abstract bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other);
    public override string ToString() => Description;
}

public abstract class CharacterTargets
{
    /// <summary>Returns true if this CharacterTargets is targeting the given character</summary>
    public abstract bool IsTargeted(CharacterIdentifier character);

    public CharacterIdentifier[] AllTargets
    {
        get => GameInstance.Get<GI_CharacterReferencer>().activeCharacterComponents
            .Select((c) => c.Identifier)
            .NotNull()
            .Where((id) => IsTargeted(id))
            .ToArray();
    }
}
public class CharacterTargetsFromUser : CharacterTargets
{
    public EffectActionTarget TargetType { get; private set; }
    public CharacterIdentifier User { get; private set; }
    public CharacterTargetsFromUser(EffectActionTarget targetType, CharacterIdentifier user)
    {
        TargetType = targetType;
        User = user;
    }
    public override bool IsTargeted(CharacterIdentifier character) => TargetType.IsTargeted(User, character);

    /// <summary>Pass in the type of target you want, and the user that is doing the targeting, and will return the associated CharacterTargets
    /// <br/> - Example: CharacterTargets.GetFrom<TargetNearestEnemy>(user)</summary>
    public static CharacterTargets GetFrom<TTarget>(CharacterIdentifier user)
        where TTarget : EffectActionTarget, new()
        => new TTarget().GetTargetsFrom(user);
}
public class CharacterTargetsFromFunc : CharacterTargets
{
    private Func<CharacterIdentifier, bool> filterFunc;
    public CharacterTargetsFromFunc(Func<CharacterIdentifier, bool> filterFunc)
    {
        this.filterFunc = filterFunc;
    }
    public override bool IsTargeted(CharacterIdentifier character) => filterFunc.Invoke(character);

    /// <summary>Pass in the type of target you want, and the user that is doing the targeting, and will return the associated CharacterTargets
    /// <br/> - Example: CharacterTargets.GetFrom<TargetNearestEnemy>(user)</summary>
    public static CharacterTargets GetFrom<TTarget>(CharacterIdentifier user)
        where TTarget : EffectActionTarget, new()
        => new TTarget().GetTargetsFrom(user);
}

// ----------------------------
// TARGET TYPES BELOW!!!!!
// ----------------------------

[Serializable]
public class TargetAll : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) => true;
    public override string Description => "all";
}

[Serializable]
public class TargetSelf : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) => user == other;
    public override string Description => "self";
}
[Serializable]
public class TargetPlayer : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) => other.IsPlayer();
    public override string Description => "autumn";
}
[Serializable]
public class TargetEnemies : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) 
        => other.TemplateCreatedFrom.characterTags.Contains(CharacterTags.Enemy);
    public override string Description => "all enemies";
}
[Serializable]
public class TargetObstacles : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other)
        => other.TemplateCreatedFrom.characterTags.Contains(CharacterTags.Obstacle);
    public override string Description => "all obstacles";
}

[Serializable]
public class TargetBattleAdjacent : EffectActionTarget
{
    public bool includeSelf = false;
    public bool includeDiagonal = false;
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other)
    {
        //Check for self target
        if (user == other && includeSelf) return true;

        //Ensure these are Char_Battle characters
        if (user.Stats.owner is not Char_Battle battle_user) return false;
        if (other.Stats.owner is not Char_Battle battle_other) return false;

        //Get difference in position
        Vector2Int positionDelta = battle_other.gridPawnController.position - battle_user.gridPawnController.position;

        //Check if cardinally adjacent
        if (positionDelta.magnitude <= 1) return true;

        //Check if diagonally adjacent 
        if (includeDiagonal && positionDelta.magnitude <= Vector2Int.one.magnitude) return true;

        //If all fails, you are not adjacent
        return false;
    }
    public override string Description => "adjacent enemy";
}

[Serializable]
public class TargetHasTag : EffectActionTarget
{
    public CharacterTags tag;
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) =>
        other.TemplateCreatedFrom.characterTags.Contains(tag);
    public override string Description => tag.ToString();
}
[Serializable]
public class TargetIsFromTemplate : EffectActionTarget
{
    public CharacterTemplate template;
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) =>
        other.TemplateCreatedFrom.UniqueID == template.UniqueID;
    public override string Description => template.characterName;
}

[Serializable]
public class MultiTargets_AND : EffectActionTarget
{
    [Box, Polymorphic, SerializeReference] public EffectActionTarget[] targets;

    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other)
    {
        foreach (var target in targets) 
            if (!target.IsTargeted(user, other))
                return false;
        return true;
    }
    public override string Description { get 
        {
            int count = targets.Count();
            if (count == 1) return targets[0].Description;
            if (count == 2) return targets[0].Description + " and " + targets[1].Description;

            string desc = "";
            for (int i = 0; i < count - 1; i++)
                desc += targets[i].Description + ", ";

            desc += "and " + targets[count - 1];

            return desc;
        } 
    }
}
[Serializable]
public class MultiTargets_OR : EffectActionTarget
{
    [Box, Polymorphic, SerializeReference] public EffectActionTarget[] targets;

    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other)
    {
        foreach (var target in targets)
            if (target.IsTargeted(user, other))
                return true;
        return false;
    }
    public override string Description
    {
        get
        {
            int count = targets.Count();
            if (count == 1) return targets[0].Description;
            if (count == 2) return targets[0].Description + " or " + targets[1].Description;

            string desc = "";
            for (int i = 0; i < count - 1; i++)
                desc += targets[i].Description + ", ";

            desc += "or " + targets[count - 1];

            return desc;
        }
    }
}
