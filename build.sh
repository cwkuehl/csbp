# Installation des SDKs:
# sudo snap install dotnet-sdk --channel=5.0/stable --classic
# sudo snap alias dotnet-sdk.dotnet dotnet
# ==> Beim Starten fehlen gtk-DLLs.
# sudo wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
# sudo dpkg -i packages-microsoft-prod.deb
# sudo apt-get update
# sudo apt-get install -y apt-transport-https
# sudo apt-get update
# sudo apt-get install -y dotnet-sdk-5.0
# ==> OK
dotnet publish ~/cs/csbp/CSBP/CSBP.csproj --configuration Release --framework net5.0 -p:Version=1.0.0.0

cp -rf /home/wolfgang/cs/csbp/CSBP/bin/Release/net5.0/publish/ /opt/Haushalt/CSBP/
cp -rf /home/wolfgang/cs/csbp/CSBP/Resources/ /opt/Haushalt/CSBP/publish
rm /opt/Haushalt/CSBP/publish/Resources/T*
rm /opt/Haushalt/CSBP/publish/Resources/M*
