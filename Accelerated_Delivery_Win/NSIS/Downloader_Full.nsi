!define APP_NAME "Accelerated Delivery"
!define COMP_NAME "Two Button Crew"
!define VERSION "00.6.00.00"
!define COPYRIGHT "Two Button Crew  © 2010-2012"
!define DESCRIPTION "A game by Two Button Crew"
!define INSTALLER_NAME "C:\Users\Alex\Downloads\Nsisqssg\Output\Accelerated Delivery\AD_full_web.exe"

SetCompressor LZMA
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
;InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "C:\TEMP"

VIProductVersion  "${VERSION}"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

!include "MUI.nsh"
!insertmacro MUI_PAGE_INSTFILES

Section -MainPrograms
	connected:
	SetOverwrite ifnewer
	SetOutPath "C:\TEMP"
	NSISdl::download "http://www.accelerateddeliverygame.com/assets/AD_full.exe" "setup.exe"
	Pop $0
	StrCmp $0 "success" execute failure
	execute:
		Exec '"C:\TEMP\setup.exe"'
		goto done
	failure:
		StrCmp $0 "cancel" done error
	error:
		MessageBox MB_OK "Download failed: $0"
		goto done
	done:
		Quit
SectionEnd
