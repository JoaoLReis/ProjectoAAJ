PUSHD ".\Master\bin\Debug"
start Master.exe
POPD 2
PUSHD ".\Server\bin\Debug"
start Server.exe 2001
start Server.exe 2002
