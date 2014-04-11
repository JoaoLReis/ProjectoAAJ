PUSHD ".\Master\bin\Debug"
start Master.exe
POPD 2
PUSHD ".\Server\bin\Debug"
start Server.exe 1001
start Server.exe 1002
start Server.exe 1003
start Server.exe 1004
start Server.exe 1005
POPD 2
PUSHD ".\Client\bin\Debug"
start Client
start Client
start Client
