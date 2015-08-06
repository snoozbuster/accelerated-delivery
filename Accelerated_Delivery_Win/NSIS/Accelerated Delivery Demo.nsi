############################################################################################
#      NSIS Installation Script created by NSIS Quick Setup Script Generator v1.09.18
#               Entirely Edited with NullSoft Scriptable Installation System                
#              by Vlasis K. Barkas aka Red Wine red_wine@freemail.gr Sep 2006               
############################################################################################

!define APP_NAME "Accelerated Delivery Demo"
!define COMP_NAME "Two Button Crew"
!define WEB_SITE "http://www.accelerateddeliverygame.com"
!define VERSION "1.00.00.00"
!define COPYRIGHT "Two Button Crew � 2013"
!define DESCRIPTION "Do you have what it takes to handle with care?"
!define INSTALLER_NAME "C:\Users\Alex\Downloads\Nsisqssg\Output\Accelerated Delivery\AD_demo_full.exe"
!define MAIN_APP_EXE "Accelerated_Delivery_Demo.exe"
!define INSTALL_TYPE "SetShellVarContext current"
!define REG_ROOT "HKCU"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

!define REG_START_MENU "Start Menu Folder"

var SM_Folder

######################################################################

VIProductVersion  "${VERSION}"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

######################################################################

SetCompressor ZLIB
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\Accelerated Delivery Demo"

######################################################################

!include "MUI.nsh"

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!insertmacro MUI_PAGE_DIRECTORY

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Accelerated Delivery Demo"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${REG_START_MENU}"
!insertmacro MUI_PAGE_STARTMENU Application $SM_Folder
!endif

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAIN_APP_EXE}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

######################################################################

Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Accelerated_Delivery.dll"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Accelerated_Delivery_Demo.exe"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\BEPUphysics.dll"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\DPSF.dll"
SetOutPath "$INSTDIR\Content\Video"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Video\level select.wmv"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Video\level select.xnb"
SetOutPath "$INSTDIR\Content\textures"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\blackTex.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\cardboardBox_01_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\cardboardBox_02_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\cardboardBox_03_1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\cardboardbox_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\circuitboard_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\circuitboard_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\cloudstest2_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\DispenserTex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\DispenserTubeTex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel00_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel00_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel01_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel01_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel02_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel02_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel03_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\dlevel03_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\FlowerBurst.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\icenor_01_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\icetexalpha_01_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\icetex_01.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\icetex_01_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\ice_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\ice_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\lava.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\Lazar.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\Lazar_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level01_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level01_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level01_machine_tex_1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level02_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level02_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level03_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level03_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level04_machine_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\level04_machine_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\lightMap.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\metal.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\noise.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\noise_nor.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\rubber_belt_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\rubber_belt_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\rubber_belt_nor_1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\Skybox_Lavatheme_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\skydome1_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\SkyDomeIce_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\Stripes_01_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\tabletop_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\volcano_nor_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\volcano_tex_0.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\textures\waterbump.xnb"
SetOutPath "$INSTDIR\Content\Shaders"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Shaders\bbEffect.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Shaders\ice.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Shaders\laser.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Shaders\lava.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Shaders\shadowmap.xnb"
SetOutPath "$INSTDIR\Content\Music\Voice Acting"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\HandleWithCare.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level00.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level01.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level02.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level03.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level04.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level05.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level06.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level07.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level08.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level09.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level10-5.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\Voice Acting\Level10.xnb"
SetOutPath "$INSTDIR\Content\Music\SFX"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Achievement.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Box_Death.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Button_Depress.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\button_depress_2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Button_Release.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Button_Roll_Over.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Close_metal_door.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Explosion.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Fail.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Laser.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Level_Win.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_00.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_01.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_02.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_02_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_02_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_03.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_03_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_03_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_04.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_04_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_04_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_05.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_05_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_05_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_06.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_06_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_06_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_07.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_07_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_07_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_08.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_08_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_08_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_09.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_09_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_09_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_10.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_10_intro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Machine_Sound_10_outro.wav"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Pause_Jingle.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Possible_Box_Success.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Possible_Result_Da.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Press_Start.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\Siren.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\SFX\startup.xnb"
SetOutPath "$INSTDIR\Content\Music\AD_Music"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\AD_Music\Generic Loop.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\AD_Music\Ice Intro.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\AD_Music\Ice Loop.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\AD_Music\Lava Loop.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\AD_Music\Menu Intro.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Music\AD_Music\Menu Loop.xnb"
SetOutPath "$INSTDIR\Content\LevelD3"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass3.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass4.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass5.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass6.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\glass7.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine1_box.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine3.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine3_glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine3_glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine4.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine5.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine5_glass.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD3\machine6.xnb"
SetOutPath "$INSTDIR\Content\LevelD2"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\auto_switcher.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\glass3.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\glass4.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\glass5.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\glass6.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine1_glass.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine2_base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine2_door.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine2_glass.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine2_wheels.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine3_bucket.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine3_glass.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine4_base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine4_glass.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD2\machine4_stripes.xnb"
SetOutPath "$INSTDIR\Content\LevelD1"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine1_minus_x.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine1_minus_y.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine1_plus_x.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine1_plus_y.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine1_rotatebase.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine2_auto.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\LevelD1\machine2_glass.xnb"
SetOutPath "$INSTDIR\Content\Level4"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\extras.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\glass3.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\glass4.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\glass5.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\machine1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\machine2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\machine3_part1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\machine3_part2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\machine4_part1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level4\machine4_part2.xnb"
SetOutPath "$INSTDIR\Content\Level3"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\flags.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\machine1_part1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\machine1_part2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\machine1_part3.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\machine2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\machine2_stripes.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level3\machine3.xnb"
SetOutPath "$INSTDIR\Content\Level2"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\machine1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\machine1_stripes.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\machine2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level2\machine2_stripes.xnb"
SetOutPath "$INSTDIR\Content\Level1"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level1\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level1\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level1\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level1\machine1_auto.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level1\machine1_glass.xnb"
SetOutPath "$INSTDIR\Content\Level0"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level0\base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level0\glass1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level0\glass2.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level0\machine1.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Level0\machine1_glass.xnb"
SetOutPath "$INSTDIR\Content\Font"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\Ad-Font.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\AD-Font_big.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\AD-Font_small.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\CrashFont.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\KeyboardFinal_reach.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\LCD numbers.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\LCD.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\press start.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\scoreboard.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\UI base.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\Font\xboxbuttons_final.xnb"
SetOutPath "$INSTDIR\Content\All Levels"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\beach_plane.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\black_box.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\blue_box.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\box.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\dispenser.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\generic_outer.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\ice_plane.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\outer_beach.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\outer_ice.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\outer_lava.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\outer_sky.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\outer_space.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\plane.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\skybox_beach.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\skybox_ice.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\skybox_lava.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\skybox_sky.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\skybox_space.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\sky_plane.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\space_place.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\All Levels\tube_x.xnb"
SetOutPath "$INSTDIR\Content\2D\Splashes and Overlays"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Splashes and Overlays\Background.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Splashes and Overlays\box_720x400_light.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Splashes and Overlays\BurningBoxesLogo01demo.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Splashes and Overlays\ending_splash.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Splashes and Overlays\Logo.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Splashes and Overlays\spawnBar.xnb"
SetOutPath "$INSTDIR\Content\2D\Special Text"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Special Text\level.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Special Text\words.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Special Text\words2.xnb"
SetOutPath "$INSTDIR\Content\2D\Other"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_01.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_01_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_02.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_02_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_03.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_03_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_04.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_04_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_05.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_05_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_06.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\num_06_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Other\plus1.xnb"
SetOutPath "$INSTDIR\Content\2D\Options Menu"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Options Menu\arrows.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Options Menu\borders.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Options Menu\DifficultySlider.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Options Menu\options_fat.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Options Menu\options_widescr.xnb"
SetOutPath "$INSTDIR\Content\2D\Music Player"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Music Player\music.xnb"
SetOutPath "$INSTDIR\Content\2D\Level Select"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\box_440x260_light.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\icons.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\icons_active.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\lock_overlay.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\question_mark.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\selection-glow.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Level Select\stars.xnb"
SetOutPath "$INSTDIR\Content\2D\Buttons"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Buttons\buttons.xnb"
File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Demo\v1.0 - Final\Content\2D\Buttons\save_selection.xnb"
Call CheckXNA4
Call SetupDotNetSectionIfNeeded
SectionEnd

######################################################################

Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\uninstall.exe"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$SM_Folder"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!insertmacro MUI_STARTMENU_WRITE_END
!endif

!ifndef REG_START_MENU
CreateDirectory "$SMPROGRAMS\Accelerated Delivery Demo"
CreateShortCut "$SMPROGRAMS\Accelerated Delivery Demo\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\Accelerated Delivery Demo\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!endif

WriteRegStr ${REG_ROOT} "${REG_APP_PATH}" "" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\uninstall.exe"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "Publisher" "${COMP_NAME}"

!ifdef WEB_SITE
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "URLInfoAbout" "${WEB_SITE}"
!endif
SectionEnd

######################################################################

Section Uninstall
${INSTALL_TYPE}
Delete "$INSTDIR\Accelerated_Delivery.dll"
Delete "$INSTDIR\Accelerated_Delivery_Demo.exe"
Delete "$INSTDIR\BEPUphysics.dll"
Delete "$INSTDIR\DPSF.dll"
Delete "$INSTDIR\Content\Video\level select.wmv"
Delete "$INSTDIR\Content\Video\level select.xnb"
Delete "$INSTDIR\Content\textures\blackTex.xnb"
Delete "$INSTDIR\Content\textures\cardboardBox_01_0.xnb"
Delete "$INSTDIR\Content\textures\cardboardBox_02_0.xnb"
Delete "$INSTDIR\Content\textures\cardboardBox_03_1.xnb"
Delete "$INSTDIR\Content\textures\cardboardbox_nor_0.xnb"
Delete "$INSTDIR\Content\textures\circuitboard_nor_0.xnb"
Delete "$INSTDIR\Content\textures\circuitboard_tex_0.xnb"
Delete "$INSTDIR\Content\textures\cloudstest2_0.xnb"
Delete "$INSTDIR\Content\textures\DispenserTex_0.xnb"
Delete "$INSTDIR\Content\textures\DispenserTubeTex_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel00_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel00_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel01_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel01_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel02_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel02_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel03_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\dlevel03_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\FlowerBurst.xnb"
Delete "$INSTDIR\Content\textures\icenor_01_0.xnb"
Delete "$INSTDIR\Content\textures\icetexalpha_01_0.xnb"
Delete "$INSTDIR\Content\textures\icetex_01.xnb"
Delete "$INSTDIR\Content\textures\icetex_01_0.xnb"
Delete "$INSTDIR\Content\textures\ice_nor_0.xnb"
Delete "$INSTDIR\Content\textures\ice_tex_0.xnb"
Delete "$INSTDIR\Content\textures\lava.xnb"
Delete "$INSTDIR\Content\textures\Lazar.xnb"
Delete "$INSTDIR\Content\textures\Lazar_0.xnb"
Delete "$INSTDIR\Content\textures\level01_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\level01_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\level01_machine_tex_1.xnb"
Delete "$INSTDIR\Content\textures\level02_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\level02_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\level03_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\level03_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\level04_machine_nor_0.xnb"
Delete "$INSTDIR\Content\textures\level04_machine_tex_0.xnb"
Delete "$INSTDIR\Content\textures\lightMap.xnb"
Delete "$INSTDIR\Content\textures\metal.xnb"
Delete "$INSTDIR\Content\textures\noise.xnb"
Delete "$INSTDIR\Content\textures\noise_nor.xnb"
Delete "$INSTDIR\Content\textures\rubber_belt_0.xnb"
Delete "$INSTDIR\Content\textures\rubber_belt_nor_0.xnb"
Delete "$INSTDIR\Content\textures\rubber_belt_nor_1.xnb"
Delete "$INSTDIR\Content\textures\Skybox_Lavatheme_0.xnb"
Delete "$INSTDIR\Content\textures\skydome1_0.xnb"
Delete "$INSTDIR\Content\textures\SkyDomeIce_0.xnb"
Delete "$INSTDIR\Content\textures\Stripes_01_0.xnb"
Delete "$INSTDIR\Content\textures\tabletop_0.xnb"
Delete "$INSTDIR\Content\textures\volcano_nor_0.xnb"
Delete "$INSTDIR\Content\textures\volcano_tex_0.xnb"
Delete "$INSTDIR\Content\textures\waterbump.xnb"
Delete "$INSTDIR\Content\Shaders\bbEffect.xnb"
Delete "$INSTDIR\Content\Shaders\ice.xnb"
Delete "$INSTDIR\Content\Shaders\laser.xnb"
Delete "$INSTDIR\Content\Shaders\lava.xnb"
Delete "$INSTDIR\Content\Shaders\shadowmap.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\HandleWithCare.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level00.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level01.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level02.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level03.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level04.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level05.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level06.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level07.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level08.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level09.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level10-5.xnb"
Delete "$INSTDIR\Content\Music\Voice Acting\Level10.xnb"
Delete "$INSTDIR\Content\Music\SFX\Achievement.xnb"
Delete "$INSTDIR\Content\Music\SFX\Box_Death.xnb"
Delete "$INSTDIR\Content\Music\SFX\Button_Depress.xnb"
Delete "$INSTDIR\Content\Music\SFX\button_depress_2.xnb"
Delete "$INSTDIR\Content\Music\SFX\Button_Release.xnb"
Delete "$INSTDIR\Content\Music\SFX\Button_Roll_Over.xnb"
Delete "$INSTDIR\Content\Music\SFX\Close_metal_door.xnb"
Delete "$INSTDIR\Content\Music\SFX\Explosion.xnb"
Delete "$INSTDIR\Content\Music\SFX\Fail.xnb"
Delete "$INSTDIR\Content\Music\SFX\Laser.xnb"
Delete "$INSTDIR\Content\Music\SFX\Level_Win.xnb"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_00.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_01.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_02.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_02_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_02_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_03.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_03_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_03_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_04.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_04_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_04_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_05.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_05_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_05_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_06.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_06_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_06_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_07.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_07_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_07_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_08.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_08_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_08_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_09.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_09_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_09_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_10.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_10_intro.wav"
Delete "$INSTDIR\Content\Music\SFX\Machine_Sound_10_outro.wav"
Delete "$INSTDIR\Content\Music\SFX\Pause_Jingle.xnb"
Delete "$INSTDIR\Content\Music\SFX\Possible_Box_Success.xnb"
Delete "$INSTDIR\Content\Music\SFX\Possible_Result_Da.xnb"
Delete "$INSTDIR\Content\Music\SFX\Press_Start.xnb"
Delete "$INSTDIR\Content\Music\SFX\Siren.xnb"
Delete "$INSTDIR\Content\Music\SFX\startup.xnb"
Delete "$INSTDIR\Content\Music\AD_Music\Generic Loop.xnb"
Delete "$INSTDIR\Content\Music\AD_Music\Ice Intro.xnb"
Delete "$INSTDIR\Content\Music\AD_Music\Ice Loop.xnb"
Delete "$INSTDIR\Content\Music\AD_Music\Lava Loop.xnb"
Delete "$INSTDIR\Content\Music\AD_Music\Menu Intro.xnb"
Delete "$INSTDIR\Content\Music\AD_Music\Menu Loop.xnb"
Delete "$INSTDIR\Content\LevelD3\base.xnb"
Delete "$INSTDIR\Content\LevelD3\glass1.xnb"
Delete "$INSTDIR\Content\LevelD3\glass2.xnb"
Delete "$INSTDIR\Content\LevelD3\glass3.xnb"
Delete "$INSTDIR\Content\LevelD3\glass4.xnb"
Delete "$INSTDIR\Content\LevelD3\glass5.xnb"
Delete "$INSTDIR\Content\LevelD3\glass6.xnb"
Delete "$INSTDIR\Content\LevelD3\glass7.xnb"
Delete "$INSTDIR\Content\LevelD3\machine1.xnb"
Delete "$INSTDIR\Content\LevelD3\machine1_box.xnb"
Delete "$INSTDIR\Content\LevelD3\machine2.xnb"
Delete "$INSTDIR\Content\LevelD3\machine3.xnb"
Delete "$INSTDIR\Content\LevelD3\machine3_glass1.xnb"
Delete "$INSTDIR\Content\LevelD3\machine3_glass2.xnb"
Delete "$INSTDIR\Content\LevelD3\machine4.xnb"
Delete "$INSTDIR\Content\LevelD3\machine5.xnb"
Delete "$INSTDIR\Content\LevelD3\machine5_glass.xnb"
Delete "$INSTDIR\Content\LevelD3\machine6.xnb"
Delete "$INSTDIR\Content\LevelD2\auto_switcher.xnb"
Delete "$INSTDIR\Content\LevelD2\base.xnb"
Delete "$INSTDIR\Content\LevelD2\glass1.xnb"
Delete "$INSTDIR\Content\LevelD2\glass2.xnb"
Delete "$INSTDIR\Content\LevelD2\glass3.xnb"
Delete "$INSTDIR\Content\LevelD2\glass4.xnb"
Delete "$INSTDIR\Content\LevelD2\glass5.xnb"
Delete "$INSTDIR\Content\LevelD2\glass6.xnb"
Delete "$INSTDIR\Content\LevelD2\machine1_glass.xnb"
Delete "$INSTDIR\Content\LevelD2\machine2_base.xnb"
Delete "$INSTDIR\Content\LevelD2\machine2_door.xnb"
Delete "$INSTDIR\Content\LevelD2\machine2_glass.xnb"
Delete "$INSTDIR\Content\LevelD2\machine2_wheels.xnb"
Delete "$INSTDIR\Content\LevelD2\machine3_bucket.xnb"
Delete "$INSTDIR\Content\LevelD2\machine3_glass.xnb"
Delete "$INSTDIR\Content\LevelD2\machine4_base.xnb"
Delete "$INSTDIR\Content\LevelD2\machine4_glass.xnb"
Delete "$INSTDIR\Content\LevelD2\machine4_stripes.xnb"
Delete "$INSTDIR\Content\LevelD1\base.xnb"
Delete "$INSTDIR\Content\LevelD1\glass1.xnb"
Delete "$INSTDIR\Content\LevelD1\glass2.xnb"
Delete "$INSTDIR\Content\LevelD1\machine1_minus_x.xnb"
Delete "$INSTDIR\Content\LevelD1\machine1_minus_y.xnb"
Delete "$INSTDIR\Content\LevelD1\machine1_plus_x.xnb"
Delete "$INSTDIR\Content\LevelD1\machine1_plus_y.xnb"
Delete "$INSTDIR\Content\LevelD1\machine1_rotatebase.xnb"
Delete "$INSTDIR\Content\LevelD1\machine2_auto.xnb"
Delete "$INSTDIR\Content\LevelD1\machine2_glass.xnb"
Delete "$INSTDIR\Content\Level4\base.xnb"
Delete "$INSTDIR\Content\Level4\extras.xnb"
Delete "$INSTDIR\Content\Level4\glass1.xnb"
Delete "$INSTDIR\Content\Level4\glass2.xnb"
Delete "$INSTDIR\Content\Level4\glass3.xnb"
Delete "$INSTDIR\Content\Level4\glass4.xnb"
Delete "$INSTDIR\Content\Level4\glass5.xnb"
Delete "$INSTDIR\Content\Level4\machine1.xnb"
Delete "$INSTDIR\Content\Level4\machine2.xnb"
Delete "$INSTDIR\Content\Level4\machine3_part1.xnb"
Delete "$INSTDIR\Content\Level4\machine3_part2.xnb"
Delete "$INSTDIR\Content\Level4\machine4_part1.xnb"
Delete "$INSTDIR\Content\Level4\machine4_part2.xnb"
Delete "$INSTDIR\Content\Level3\base.xnb"
Delete "$INSTDIR\Content\Level3\flags.xnb"
Delete "$INSTDIR\Content\Level3\glass1.xnb"
Delete "$INSTDIR\Content\Level3\machine1_part1.xnb"
Delete "$INSTDIR\Content\Level3\machine1_part2.xnb"
Delete "$INSTDIR\Content\Level3\machine1_part3.xnb"
Delete "$INSTDIR\Content\Level3\machine2.xnb"
Delete "$INSTDIR\Content\Level3\machine2_stripes.xnb"
Delete "$INSTDIR\Content\Level3\machine3.xnb"
Delete "$INSTDIR\Content\Level2\base.xnb"
Delete "$INSTDIR\Content\Level2\glass1.xnb"
Delete "$INSTDIR\Content\Level2\glass2.xnb"
Delete "$INSTDIR\Content\Level2\machine1.xnb"
Delete "$INSTDIR\Content\Level2\machine1_stripes.xnb"
Delete "$INSTDIR\Content\Level2\machine2.xnb"
Delete "$INSTDIR\Content\Level2\machine2_stripes.xnb"
Delete "$INSTDIR\Content\Level1\base.xnb"
Delete "$INSTDIR\Content\Level1\glass1.xnb"
Delete "$INSTDIR\Content\Level1\glass2.xnb"
Delete "$INSTDIR\Content\Level1\machine1_auto.xnb"
Delete "$INSTDIR\Content\Level1\machine1_glass.xnb"
Delete "$INSTDIR\Content\Level0\base.xnb"
Delete "$INSTDIR\Content\Level0\glass1.xnb"
Delete "$INSTDIR\Content\Level0\glass2.xnb"
Delete "$INSTDIR\Content\Level0\machine1.xnb"
Delete "$INSTDIR\Content\Level0\machine1_glass.xnb"
Delete "$INSTDIR\Content\Font\Ad-Font.xnb"
Delete "$INSTDIR\Content\Font\AD-Font_big.xnb"
Delete "$INSTDIR\Content\Font\AD-Font_small.xnb"
Delete "$INSTDIR\Content\Font\CrashFont.xnb"
Delete "$INSTDIR\Content\Font\KeyboardFinal_reach.xnb"
Delete "$INSTDIR\Content\Font\LCD numbers.xnb"
Delete "$INSTDIR\Content\Font\LCD.xnb"
Delete "$INSTDIR\Content\Font\press start.xnb"
Delete "$INSTDIR\Content\Font\scoreboard.xnb"
Delete "$INSTDIR\Content\Font\UI base.xnb"
Delete "$INSTDIR\Content\Font\xboxbuttons_final.xnb"
Delete "$INSTDIR\Content\All Levels\beach_plane.xnb"
Delete "$INSTDIR\Content\All Levels\black_box.xnb"
Delete "$INSTDIR\Content\All Levels\blue_box.xnb"
Delete "$INSTDIR\Content\All Levels\box.xnb"
Delete "$INSTDIR\Content\All Levels\dispenser.xnb"
Delete "$INSTDIR\Content\All Levels\generic_outer.xnb"
Delete "$INSTDIR\Content\All Levels\ice_plane.xnb"
Delete "$INSTDIR\Content\All Levels\outer_beach.xnb"
Delete "$INSTDIR\Content\All Levels\outer_ice.xnb"
Delete "$INSTDIR\Content\All Levels\outer_lava.xnb"
Delete "$INSTDIR\Content\All Levels\outer_sky.xnb"
Delete "$INSTDIR\Content\All Levels\outer_space.xnb"
Delete "$INSTDIR\Content\All Levels\plane.xnb"
Delete "$INSTDIR\Content\All Levels\skybox_beach.xnb"
Delete "$INSTDIR\Content\All Levels\skybox_ice.xnb"
Delete "$INSTDIR\Content\All Levels\skybox_lava.xnb"
Delete "$INSTDIR\Content\All Levels\skybox_sky.xnb"
Delete "$INSTDIR\Content\All Levels\skybox_space.xnb"
Delete "$INSTDIR\Content\All Levels\sky_plane.xnb"
Delete "$INSTDIR\Content\All Levels\space_place.xnb"
Delete "$INSTDIR\Content\All Levels\tube_x.xnb"
Delete "$INSTDIR\Content\2D\Splashes and Overlays\Background.xnb"
Delete "$INSTDIR\Content\2D\Splashes and Overlays\box_720x400_light.xnb"
Delete "$INSTDIR\Content\2D\Splashes and Overlays\BurningBoxesLogo01demo.xnb"
Delete "$INSTDIR\Content\2D\Splashes and Overlays\ending_splash.xnb"
Delete "$INSTDIR\Content\2D\Splashes and Overlays\Logo.xnb"
Delete "$INSTDIR\Content\2D\Splashes and Overlays\spawnBar.xnb"
Delete "$INSTDIR\Content\2D\Special Text\level.xnb"
Delete "$INSTDIR\Content\2D\Special Text\words.xnb"
Delete "$INSTDIR\Content\2D\Special Text\words2.xnb"
Delete "$INSTDIR\Content\2D\Other\num_01.xnb"
Delete "$INSTDIR\Content\2D\Other\num_01_active.xnb"
Delete "$INSTDIR\Content\2D\Other\num_02.xnb"
Delete "$INSTDIR\Content\2D\Other\num_02_active.xnb"
Delete "$INSTDIR\Content\2D\Other\num_03.xnb"
Delete "$INSTDIR\Content\2D\Other\num_03_active.xnb"
Delete "$INSTDIR\Content\2D\Other\num_04.xnb"
Delete "$INSTDIR\Content\2D\Other\num_04_active.xnb"
Delete "$INSTDIR\Content\2D\Other\num_05.xnb"
Delete "$INSTDIR\Content\2D\Other\num_05_active.xnb"
Delete "$INSTDIR\Content\2D\Other\num_06.xnb"
Delete "$INSTDIR\Content\2D\Other\num_06_active.xnb"
Delete "$INSTDIR\Content\2D\Other\plus1.xnb"
Delete "$INSTDIR\Content\2D\Options Menu\arrows.xnb"
Delete "$INSTDIR\Content\2D\Options Menu\borders.xnb"
Delete "$INSTDIR\Content\2D\Options Menu\DifficultySlider.xnb"
Delete "$INSTDIR\Content\2D\Options Menu\options_fat.xnb"
Delete "$INSTDIR\Content\2D\Options Menu\options_widescr.xnb"
Delete "$INSTDIR\Content\2D\Music Player\music.xnb"
Delete "$INSTDIR\Content\2D\Level Select\box_440x260_light.xnb"
Delete "$INSTDIR\Content\2D\Level Select\icons.xnb"
Delete "$INSTDIR\Content\2D\Level Select\icons_active.xnb"
Delete "$INSTDIR\Content\2D\Level Select\lock_overlay.xnb"
Delete "$INSTDIR\Content\2D\Level Select\question_mark.xnb"
Delete "$INSTDIR\Content\2D\Level Select\selection-glow.xnb"
Delete "$INSTDIR\Content\2D\Level Select\stars.xnb"
Delete "$INSTDIR\Content\2D\Buttons\buttons.xnb"
Delete "$INSTDIR\Content\2D\Buttons\save_selection.xnb"
 
RmDir "$INSTDIR\Content\2D\Buttons"
RmDir "$INSTDIR\Content\2D\Level Select"
RmDir "$INSTDIR\Content\2D\Music Player"
RmDir "$INSTDIR\Content\2D\Options Menu"
RmDir "$INSTDIR\Content\2D\Other"
RmDir "$INSTDIR\Content\2D\Special Text"
RmDir "$INSTDIR\Content\2D\Splashes and Overlays"
RmDir "$INSTDIR\Content\All Levels"
RmDir "$INSTDIR\Content\Font"
RmDir "$INSTDIR\Content\Level0"
RmDir "$INSTDIR\Content\Level1"
RmDir "$INSTDIR\Content\Level2"
RmDir "$INSTDIR\Content\Level3"
RmDir "$INSTDIR\Content\Level4"
RmDir "$INSTDIR\Content\LevelD1"
RmDir "$INSTDIR\Content\LevelD2"
RmDir "$INSTDIR\Content\LevelD3"
RmDir "$INSTDIR\Content\Music\AD_Music"
RmDir "$INSTDIR\Content\Music\SFX"
RmDir "$INSTDIR\Content\Music\Voice Acting"
RmDir "$INSTDIR\Content\Shaders"
RmDir "$INSTDIR\Content\textures"
RmDir "$INSTDIR\Content\Video"
 
Delete "$INSTDIR\uninstall.exe"
!ifdef WEB_SITE
Delete "$INSTDIR\${APP_NAME} website.url"
!endif

RmDir "$INSTDIR"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $SM_Folder
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk"
!endif
RmDir "$SMPROGRAMS\$SM_Folder"
!endif

!ifndef REG_START_MENU
Delete "$SMPROGRAMS\Accelerated Delivery Demo\${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\Accelerated Delivery Demo\${APP_NAME} Website.lnk"
!endif
RmDir "$SMPROGRAMS\Accelerated Delivery Demo"
!endif

DeleteRegKey ${REG_ROOT} "${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd

######################################################################

Function CheckXNA4
	ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\XNA\Framework\v4.0" "Installed"
	ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\XNA\Framework\v4.0" "Refresh1Installed"
	IntCmp $0 1 installed notInstalled notInstalled
	installed:
		IntCmp $1 1 allGood notInstalled notInstalled
	notInstalled:
		MessageBox MB_OK "It appears XNA Game Studio 4.0 Refresh is not installed. The installer for it will now be launched."
		SetOutPath "$INSTDIR"
		File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Binary Installers\dotNetFx40_Full_setup.exe"
		ExecWait '"msiexec" /i "$INSTDIR\xnafx40_redist.msi" /passive'
		Delete "$INSTDIR\xnafx40_redist.msi"
		goto allGood
	allGood:
FunctionEnd

!define DOT_MAJOR 4
!define DOT_MINOR 0
 !define BASE_URL http://download.microsoft.com/download
; .NET Framework
; English
!define URL_DOTNET_1033 "http://download.microsoft.com/download/5/6/2/562A10F9-C9F4-4313-A044-9C94E0A8FAC8/dotNetFx40_Client_x86_x64.exe"
;German
;!define URL_DOTNET_1031 "${BASE_URL}/4/f/3/4f3ac857-e063-45d0-9835-83894f20e808/dotnetfx.exe"
;Spanish
;!define URL_DOTNET_1034 "${BASE_URL}/8/f/0/8f023ff4-2dc1-4f10-9618-333f5b9f8040/dotnetfx.exe"
;French
;!define URL_DOTNET_1036 "${BASE_URL}/e/d/a/eda9d4ea-8ec9-4431-8efa-75391fb91421/dotnetfx.exe"
;Portuguese (Brazil)
;!define URL_DOTNET_1046 "${BASE_URL}/8/c/f/8cf55d0c-235e-4062-933c-64ffdf7e7043/dotnetfx.exe"
;Chinese (Simplified)
;!define URL_DOTNET_2052 "${BASE_URL}/7/b/9/7b90644d-1af0-42b9-b76d-a2770319a568/dotnetfx.exe"
;!define URL_DOTNET_4100 "${BASE_URL}/7/b/9/7b90644d-1af0-42b9-b76d-a2770319a568/dotnetfx.exe"
;Chinese (Traditional)
;!define URL_DOTNET_1028 "${BASE_URL}/8/2/7/827bb1ef-f5e1-4464-9788-40ef682930fd/dotnetfx.exe"
;!define URL_DOTNET_3076 "${BASE_URL}/8/2/7/827bb1ef-f5e1-4464-9788-40ef682930fd/dotnetfx.exe"
;!define URL_DOTNET_5124 "${BASE_URL}/8/2/7/827bb1ef-f5e1-4464-9788-40ef682930fd/dotnetfx.exe"
;Czech
;!define URL_DOTNET_1029 "${BASE_URL}/2/a/2/2a224db0-2e6d-4961-99ed-6f377555b1ef/dotnetfx.exe"
;Danish
;!define URL_DOTNET_1030 "${BASE_URL}/e/7/5/e755a559-025d-4282-95ae-d14a8d0b1929/dotnetfx.exe"
;Dutch
;!define URL_DOTNET_1043 "${BASE_URL}/4/6/b/46b519cb-bdd2-4701-b962-9ffaa323f40b/dotnetfx.exe"
;!define URL_DOTNET_2067 "${BASE_URL}/4/6/b/46b519cb-bdd2-4701-b962-9ffaa323f40b/dotnetfx.exe"
;Finnish
;!define URL_DOTNET_1035 "${BASE_URL}/d/a/6/da6b472c-157c-429a-98f6-6eb87fa36fd3/dotnetfx.exe"
;Greek
;!define URL_DOTNET_1032 "${BASE_URL}/5/9/8/598fb686-cd09-45cd-8b13-2a0fd814e4cc/dotnetfx.exe"
;Hungarian
;!define URL_DOTNET_1038 "${BASE_URL}/8/2/0/82093ba7-c9a4-457d-864d-bbeb1cd884d4/dotnetfx.exe"
;Italian
;!define URL_DOTNET_1040 "${BASE_URL}/1/f/a/1fa816d7-a8d6-4f15-b682-b96239e68ab7/dotnetfx.exe"
;!define URL_DOTNET_2064 "${BASE_URL}/1/f/a/1fa816d7-a8d6-4f15-b682-b96239e68ab7/dotnetfx.exe"
;Japanese
;!define URL_DOTNET_1041 "${BASE_URL}/5/b/5/5b510096-5b68-4e3f-8f9e-56fb7a80ca81/dotnetfx.exe"
;Korean
;!define URL_DOTNET_1042 "${BASE_URL}/d/2/d/d2db6a60-6fb1-4015-ae45-2fb84ec30faa/dotnetfx.exe"
;Norwegian
;!define URL_DOTNET_1044 "${BASE_URL}/b/6/6/b663aaf1-ef27-4119-8cf1-fa23888cf6a7/dotnetfx.exe"
;!define URL_DOTNET_2068 "${BASE_URL}/b/6/6/b663aaf1-ef27-4119-8cf1-fa23888cf6a7/dotnetfx.exe"
;Polish
;!define URL_DOTNET_1045 "${BASE_URL}/c/9/f/c9f672f3-c14b-4cff-9671-d419842d792d/dotnetfx.exe"
;Portuguese (Portugal)
;!define URL_DOTNET_2070 "${BASE_URL}/1/2/0/1206b231-b961-40ca-9ac2-e4ab7e92ca9b/dotnetfx.exe"
;Russian
;!define URL_DOTNET_1049 "${BASE_URL}/0/8/6/086e7824-ddad-45c0-b765-721e5e28e4c5/dotnetfx.exe"
;Swedish
;!define URL_DOTNET_1053 "${BASE_URL}/3/0/0/300b9c1c-9a26-4334-b273-8c0064ba5f2b/dotnetfx.exe"
;Turkish
;!define URL_DOTNET_1055 "${BASE_URL}/a/f/7/af738ebf-dc15-4c61-a20d-1c29306cd9bc/dotnetfx.exe"
; ... If you need one not listed above you will have to visit the Microsoft Download site,
; select the language you are after and scan the page source to obtain the link.
 
Var "LANGUAGE_DLL_TITLE"
Var "LANGUAGE_DLL_INFO"
Var "URL_DOTNET"
Var "OSLANGUAGE"
Var "DOTNET_RETURN_CODE"
 
LangString DESC_REMAINING ${LANG_ENGLISH} " (%d %s%s remaining)"
LangString DESC_PROGRESS ${LANG_ENGLISH} "%d.%01dkB/s" ;"%dkB (%d%%) of %dkB @ %d.%01dkB/s"
LangString DESC_PLURAL ${LANG_ENGLISH} "s"
LangString DESC_HOUR ${LANG_ENGLISH} "hour"
LangString DESC_MINUTE ${LANG_ENGLISH} "minute"
LangString DESC_SECOND ${LANG_ENGLISH} "second"
LangString DESC_CONNECTING ${LANG_ENGLISH} "Connecting..."
LangString DESC_DOWNLOADING ${LANG_ENGLISH} "Downloading %s"
LangString DESC_SHORTDOTNET ${LANG_ENGLISH} "Microsoft .Net Framework 4.0"
LangString DESC_LONGDOTNET ${LANG_ENGLISH} "Microsoft .Net Framework 4.0"
LangString DESC_DOTNET_DECISION ${LANG_ENGLISH} "$(DESC_SHORTDOTNET) is required.$\nIt is strongly \
  advised that you install$\n$(DESC_SHORTDOTNET) before continuing.$\nIf you choose to continue, \
  you will need to connect$\nto the internet before proceeding.$\nWould you like to continue with \
  the installation?"
LangString SEC_DOTNET ${LANG_ENGLISH} "$(DESC_SHORTDOTNET) "
LangString DESC_INSTALLING ${LANG_ENGLISH} "Installing"
LangString DESC_DOWNLOADING1 ${LANG_ENGLISH} "Downloading"
LangString DESC_DOWNLOADFAILED ${LANG_ENGLISH} "Download Failed:"
LangString ERROR_DOTNET_DUPLICATE_INSTANCE ${LANG_ENGLISH} "The $(DESC_SHORTDOTNET) Installer is \
  already running."
LangString ERROR_NOT_ADMINISTRATOR ${LANG_ENGLISH} "$(DESC_000022)"
LangString ERROR_INVALID_PLATFORM ${LANG_ENGLISH} "$(DESC_000023)"
LangString DESC_DOTNET_TIMEOUT ${LANG_ENGLISH} "The installation of the $(DESC_SHORTDOTNET) \
  has timed out."
LangString ERROR_DOTNET_INVALID_PATH ${LANG_ENGLISH} "The $(DESC_SHORTDOTNET) Installation$\n\
  was not found in the following location:$\n"
LangString ERROR_DOTNET_FATAL ${LANG_ENGLISH} "A fatal error occurred during the installation$\n\
  of the $(DESC_SHORTDOTNET)."
LangString FAILED_DOTNET_INSTALL ${LANG_ENGLISH} "The installation of $(PRODUCT_NAME) will$\n\
  continue. However, it may not function properly$\nuntil $(DESC_SHORTDOTNET)$\nis installed."
 
Function SetupDotNetSectionIfNeeded
 
  StrCpy $0 "0"
  StrCpy $1 "SOFTWARE\Microsoft\.NETFramework" ;registry entry to look in.
  StrCpy $2 0
 
  StartEnum:
    ;Enumerate the versions installed.
    EnumRegKey $3 HKLM "$1\policy" $2
 
    ;If we don't find any versions installed, it's not here.
    StrCmp $3 "" noDotNet notEmpty
 
    ;We found something.
    notEmpty:
      ;Find out if the RegKey starts with 'v'.  
      ;If it doesn't, goto the next key.
      StrCpy $4 $3 1 0
      StrCmp $4 "v" +1 goNext
      StrCpy $4 $3 1 1
 
      ;It starts with 'v'.  Now check to see how the installed major version
      ;relates to our required major version.
      ;If it's equal check the minor version, if it's greater, 
      ;we found a good RegKey.
      IntCmp $4 ${DOT_MAJOR} +1 goNext yesDotNetReg
      ;Check the minor version.  If it's equal or greater to our requested 
      ;version then we're good.
      StrCpy $4 $3 1 3
      IntCmp $4 ${DOT_MINOR} yesDotNetReg goNext yesDotNetReg
 
    goNext:
      ;Go to the next RegKey.
      IntOp $2 $2 + 1
      goto StartEnum
 
  yesDotNetReg:
    ;Now that we've found a good RegKey, let's make sure it's actually
    ;installed by getting the install path and checking to see if the 
    ;mscorlib.dll exists.
    EnumRegValue $2 HKLM "$1\policy\$3" 0
    ;$2 should equal whatever comes after the major and minor versions 
    ;(ie, v1.1.4322)
    StrCmp $2 "" noDotNet
    ReadRegStr $4 HKLM $1 "InstallRoot"
    ;Hopefully the install root isn't empty.
    StrCmp $4 "" noDotNet
    ;build the actuall directory path to mscorlib.dll.
    StrCpy $4 "$4$3.$2\mscorlib.dll"
    IfFileExists $4 yesDotNet noDotNet
 
  noDotNet:
    MessageBox MB_OK "It appears the .NET Framework 4 is not installed. The installer will now be launched."
    SetOutPath "$INSTDIR"
    File "C:\Users\Alex\Dropbox\Two Button Crew\Alex\AD builds\Binary Installers\dotNetFx40_Full_setup.exe"
    ExecWait '"$INSTDIR\dotNET4.exe" /SP- /SILENT'
    Delete "$INSTDIR\dotNET4.exe"
    goto done
 
  yesDotNet:
    ;Everything checks out.  Go on with the rest of the installation.
    goto done
 
  done:
    ;All done.
 
FunctionEnd
 
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${SECDOTNET} $(DESC_LONGDOTNET)
!insertmacro MUI_FUNCTION_DESCRIPTION_END
