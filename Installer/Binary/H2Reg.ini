
[MAIN]
DebugMode=0                   ;set to 1 to log everything - Check your resources (ship with =0)
DumpNSToLog_before=0          ;Very verbose but good for debugging - dumps all NS info to log file before Registration
DumpNSToLog_after=0           ;Very verbose but good for debugging - dumps all NS info to log file after Registration
OKtoReport_FinalRegError=0    ;Report general error at end if a registration error was logged
OKtoReport_FinalUnRegError=0  ;Report general error at end if a unregistration error was logged

;Advanced feature - These can be set then used as <userdir1> etc specifiers. They can
;also be set via the H2Reg.exe command line. Command line has preference over INI file.
UserDir1=''
UserDir2=''
UserDir3=''
UserDir4=''

; Resource Strings

[en] ; English
ErrSt_SysFileNotFound = 'Installation Error. Error reading system file or file not found.|%s'
ErrSt_MSHelp2RTNotFound = 'MS Help 2.x runtime files are not installed on this PC.'
ErrSt_NotAdminMode = 'You must be logged on as an Administrator.'
ErrSt_Extra = 'Installation/registration of Help files cannot proceed.'

Msg_Registering = 'Registering Online Documentation Files:'
Msg_UnRegistering = 'Unregistering Online Documentation Files:'
Msg_LoggingNSInfo = 'Logging Namespace Info'
Msg_Registering_Namespaces =  'Registering Namespaces'
Msg_Registering_Titles =  'Registering Titles'
Msg_Registering_Plugins =  'Registering Plug-ins'
Msg_Registering_Filters =  'Registering Filters'
Msg_UnRegistering_Namespaces =  'Unregistering Namespaces'
Msg_UnRegistering_Titles =  'Unregistering Titles'
Msg_UnRegistering_Plugins =  'Unregistering Plug-ins'
Msg_UnRegistering_Filters =  'Unregistering Filters'

Msg_Merging_Namespaces = 'Merging Help - This may take several minutes'
Msg_Merging = 'Merging "%s"'

PopupSt_FinalRegError='There were errors reported while Registering help files.||View Log file?'
PopupSt_FinalUnRegError='There were errors reported while Unregistering help files.||View Log file?'



; International Strings - Defaults to [en]
[de] ; German
[ja] ; Japanese
[fr] ; French
[es] ; Spanish
[it] ; Italian
[ko] ; Korean
[cn] ; Chinese (Simplified)
[tw] ; Chinese (Traditional)
[sv] ; Swedish
[nl] ; Dutch
[ru] ; Russian
[ar] ; Arabic
[he] ; Hebrew
[da] ; Danish
[no] ; Norwegian
[fi] ; Finnish
[pt] ; Portuguese
[br] ; Brazilian
[cs] ; Czech
[pl] ; Polish
[hu] ; Hungarian
[el] ; Greek
[tr] ; Turkish
[sl] ; Slovenian
[sk] ; Slovakian
[eu] ; Basque
[ca] ; Catalan


;--- Optionally you can place your Registration Commands in this file


;------- Register -r switch

[Reg_Namespace]
mysql.en|COL_Master.HxC|MySQL Connector/Net Documentation

[Reg_Title]
mysql.en|mysql.data|1033|mysql.data.HxS|mysql.data.HxS||||||

[Reg_Plugin]
MS.VSCC|_DEFAULT|mysql.en|_DEFAULT|
MS.VSCC.2003|_DEFAULT|mysql.en|_DEFAULT|
MS.VSIPCC+|_DEFAULT|mysql.en|_DEFAULT|

[Reg_Filter]
;mysql.en|MySQL Connector/Net Documentation|

;------- Unregister -u switch

[UnReg_Filter]
;mysql.en|MySQL Connector/Net Documentation

[UnReg_Plugin]
MS.VSCC|_DEFAULT|mysql.en|_DEFAULT|
MS.VSCC.2003|_DEFAULT|mysql.en|_DEFAULT|
MS.VSIPCC+|_DEFAULT|mysql.en|_DEFAULT|

[UnReg_Title]
mysql.en|mysql.data|1033

[UnReg_Namespace]
mysql.en


;------- Merge -m switch

[Merge_Namespace]
;<nsName>|[AUTO]
