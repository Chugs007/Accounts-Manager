msbuild /P:Configuration=Release /P:Platform="Any CPU"
compil32 /cc AccountsManagerSetup.iss
runas /user:Administrator "C:\Users\ochug\Desktop\AccountsManagerSetup.exe"