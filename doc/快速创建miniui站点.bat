echo off
set webpath=E:\git_clone\miniui_net

rem %systemroot%/system32/Inetsrv/APPCMD.exe delete site "miniui_net"
rem %systemroot%/system32/Inetsrv/APPCMD.exe delete apppool /apppool.name:"miniui_net"
%systemroot%/system32/Inetsrv/APPCMD.exe add site /name:"miniui_net"   /bindings:"http/*:8115:" /physicalPath:%webpath%
%systemroot%/system32/Inetsrv/APPCMD.exe add apppool /name:miniui_net
%systemroot%/system32/Inetsrv/APPCMD.exe set app "miniui_net"/ -applicationPool:miniui_net     
pause


