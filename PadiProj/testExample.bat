PUSHD ".\Master\bin\Debug"
start Master.exe
POPD 2
PUSHD ".\Server\bin\Debug"
start Server.exe 2001
start Server.exe 2002
start Server.exe 2003
start Server.exe 2004
start Server.exe 2005
