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
# ab .net8 RID linux-x64

dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net9.0 -r linux-x64 --self-contained false
#dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net8.0 -r linux-x64 --self-contained false
#thirdlicense --project ./CSBP/CSBP.csproj
#cd Asciidoc
#./asciidoc-csbp.sh
#cd ..

cp -rf /home/wolfgang/cs/csbp/CSBP/bin/Release/net9.0/linux-x64/publish/ /opt/Haushalt/CSBP/
#cp -rf /home/wolfgang/cs/csbp/CSBP/bin/Release/net8.0/linux-x64/publish/ /opt/Haushalt/CSBP/
#cp -rf /home/wolfgang/cs/csbp/CSBP/Resources/ /opt/Haushalt/CSBP/publish
#rm /opt/Haushalt/CSBP/publish/Resources/T*
#rm /opt/Haushalt/CSBP/publish/Resources/M*
