# Installation des SDKs:
# sudo apt-get update; \
#   sudo apt-get install -y apt-transport-https && \
#   sudo apt-get update && \
#   sudo apt-get install -y dotnet-sdk-6.0
# Keine Snaps verwendet!
# sudo snap install dotnet-sdk --channel=5.0/stable --classic
# sudo snap alias dotnet-sdk.dotnet dotnet
# sudo snap install dotnet-sdk --channel=6.0/stable --classic
# sudo snap alias dotnet-sdk.dotnet dotnet
# sudo snap install dotnet-runtime-60 --classic
# sudo snap alias dotnet-runtime-60.dotnet dotnet
#
# ==> Beim Starten fehlen gtk-DLLs.
# sudo wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
# sudo dpkg -i packages-microsoft-prod.deb
# sudo apt-get update
# sudo apt-get install -y apt-transport-https
# sudo apt-get update
# sudo apt-get install -y dotnet-sdk-5.0
# ==> OK
dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net5.0 -r ubuntu-x64 --self-contained false -p:Version=1.1
#dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net6.0 -r ubuntu-x64 --self-contained true -p:Version=1.1
#dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net5.0 -r win-x64 --self-contained true -p:Version=1.1

cp -rf /home/wolfgang/cs/csbp/CSBP/bin/Release/net5.0/ubuntu-x64/publish/ /opt/Haushalt/CSBP/
#cp -rf /home/wolfgang/cs/csbp/CSBP/Resources/ /opt/Haushalt/CSBP/publish
#rm /opt/Haushalt/CSBP/publish/Resources/T*
#rm /opt/Haushalt/CSBP/publish/Resources/M*
