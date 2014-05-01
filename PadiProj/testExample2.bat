PUSHD ".\Master\bin\Release"
start Master.exe
POPD 2
PUSHD ".\Server\bin\Release"
start Server.exe 2001
