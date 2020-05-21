msbuild -t:clean
msbuild -t:restore
msbuild /P:Configuration=Debug /P:Platform="Any CPU"
msbuild /P:Configuration=Release /P:Platform="Any CPU"
compil32 /cc AccountsManagerSetup.iss
pause