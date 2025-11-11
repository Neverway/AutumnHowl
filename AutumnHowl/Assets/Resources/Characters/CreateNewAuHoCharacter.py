import os
from distutils.dir_util import copy_tree
import pathlib
import yaml

state = ""


def Start():
    global state

    print("==============================")
    print("Autumn Howl Character Editor")
    print("==============================")
    print("Please enter a number to select")
    print("1. Create New AuHo Character")
    print("2. Rename Existing AuHo Character")
    print("3. Validate All AuHo SpriteLibs")
    choice = input()

    if choice == "1":
        state = "Create"
        return
    if choice == "2":
        state = "Rename"
        return
    if choice == "3":
        state = "Validate"
        return
    print(f" ")
    print(f"[HALT] Unknown option")
    print(f" ")
    Start()


def GetCharacterName():
    global characterName
    global source
    global destination

    print("Enter character name:")
    characterName = input()

    source = "_Template"
    destination = f"{characterName}"


def DuplicateTemplate():
    print(f"Creating character...")
    if not os.path.exists(source):
        print(f"{pathlib.Path(source).resolve()}")
        print("Template directory did not exist!")
        print("Exiting...")
        return False

    if os.path.exists(destination):
        print(f"[HALT] {pathlib.Path(destination).resolve()}")
        print("[HALT] A folder already exists here with that name!")
        print("Overwrite directory? Enter Y to confirm:")
        response = input()
        if response.lower() == "y":
            print("Overwriting...")
        else:
            print("Exiting...")
            return False

    copy_tree(source, destination)
    print(f"[OK] New character '{characterName}' created at {pathlib.Path(destination).resolve()}")
    return True


def RenameDuplicate():
    print(f"Renaming assets...")
    # _Battle prefab
    os.rename(destination+"/_Battle NAME.prefab", destination+f"/_Battle {characterName}.prefab")
    os.rename(destination+"/_Battle NAME.prefab.meta", destination+f"/_Battle {characterName}.prefab.meta")
    # _Layout prefab
    os.rename(destination+"/_Layout NAME.prefab", destination+f"/_Layout {characterName}.prefab")
    os.rename(destination+"/_Layout NAME.prefab.meta", destination+f"/_Layout {characterName}.prefab.meta")
    # _Overworld prefab
    os.rename(destination+"/_Overworld NAME.prefab", destination+f"/_Overworld {characterName}.prefab")
    os.rename(destination+"/_Overworld NAME.prefab.meta", destination+f"/_Overworld {characterName}.prefab.meta")
    # Battle data
    os.rename(destination+"/Battle NAME.asset", destination+f"/Battle {characterName}.asset")
    os.rename(destination+"/Battle NAME.asset.meta", destination+f"/Battle {characterName}.asset.meta")
    # Character template
    os.rename(destination+"/Char NAME.asset", destination+f"/Char {characterName}.asset")
    os.rename(destination+"/Char NAME.asset.meta", destination+f"/Char {characterName}.asset.meta")
    # Voice
    os.rename(destination+"/CharVoice NAME.asset", destination+f"/CharVoice {characterName}.asset")
    os.rename(destination+"/CharVoice NAME.asset.meta", destination+f"/CharVoice {characterName}.asset.meta")
    # Loot table
    os.rename(destination+"/Loot NAME.asset", destination+f"/Loot {characterName}.asset")
    os.rename(destination+"/Loot NAME.asset.meta", destination+f"/Loot {characterName}.asset.meta")
    # Sprite sheet
    os.rename(destination+"/Spr_NAME.png", destination+f"/Spr_{characterName}.png")
    os.rename(destination+"/Spr_NAME.png.meta", destination+f"/Spr_{characterName}.png.meta")
    # Sprite library
    os.rename(destination+"/SprLib_NAME.spriteLib", destination+f"/SprLib_{characterName}.spriteLib")
    os.rename(destination+"/SprLib_NAME.spriteLib.meta", destination+f"/SprLib_{characterName}.spriteLib.meta")
    print(f"[OK] Finished renaming assets")


def AssignSpriteMetadata():
    print(f"Assigning sprite data...")
    # Open the file
    target = destination+f"/Spr_{characterName}.png.meta"
    with open(target, "r") as file:
        contents = file.read()
    # Replace the text
    contents = contents.replace("NAME", characterName)
    # Save the file
    target = destination+f"/Spr_{characterName}.png.meta"
    with open(target, "w") as file:
        file.write(contents)
    print(f"[OK] Finished assigning sprite data")


def AssignSpriteLibraryOverrides():
    print(f"Assigning sprite library...")

    # Get the sprite meta file
    target = destination+f"/Spr_{characterName}.png.meta"
    with open(target, 'r') as f:
        data = yaml.load(f, Loader=yaml.SafeLoader)

    print(data)
    print(data.get('TextureImporter'))

    print(f"[OK] Finished assigning sprite library")


def CreateNewAuhoCharacter():
    GetCharacterName()
    if DuplicateTemplate():
        RenameDuplicate()
        AssignSpriteMetadata()
        AssignSpriteLibraryOverrides()


Start()

if state == "Create":
    CreateNewAuhoCharacter()

print(f"[COMPLETE]")
