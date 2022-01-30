echo "Zip program CSBP (c) 2022 cwkuehl.de"
rm -rf csbp
mkdir csbp
cp -a linux/. ../CSBP/Resources/Icons/WKHH.gif csbp
cp -a ../CSBP/Data/csbp0.db csbp/EmptyCsbp.db
cp -a ../CSBP/bin/Release/net5.0/ubuntu-x64/publish csbp
rm ./csbp-net5-ubuntu-x64.zip
zip -r csbp-net5-ubuntu-x64.zip csbp
