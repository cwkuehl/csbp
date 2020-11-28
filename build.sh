# sudo snap install dotnet-sdk --channel=5.0/stable --classic
dotnet publish ~/cs/csbp/CSBP/CSBP.csproj --configuration Release --framework net5.0 -p:Version=1.0.0.0

cp -rf /home/wolfgang/cs/csbp/CSBP/bin/Release/net5.0/publish/ /opt/Haushalt/CSBP/
cp -rf /home/wolfgang/cs/csbp/CSBP/Resources/ /opt/Haushalt/CSBP/publish
rm /opt/Haushalt/CSBP/publish/Resources/T*
rm /opt/Haushalt/CSBP/publish/Resources/M*
