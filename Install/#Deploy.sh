#! /bin/bash
echo "Deploy program CSBP (c) 2022 cwkuehl.de"

build() {
  echo "$1 $2"
  rm -rf csbp
  mkdir csbp
  if [[ $1 == *"win"* ]]; then
    cp -a windows/. csbp
  else
    cp -a linux/. csbp
  fi
  cp -a ../CSBP/Resources/Icons/WKHH.gif csbp
  cp -a ../CSBP/Data/csbp0.db csbp/EmptyCsbp.db
  cp -a ../LICENSE csbp/LICENSE.TXT
  cp -a ../THIRD-PARTY-NOTICES.TXT csbp
  cp -a ../Asciidoc/Csbp-Hilfe.html csbp
  cp -a "../CSBP/bin/Release/$1/publish" csbp
  rm -f "$2"
  zip -rq "$2" csbp
}

thirdlicense --project ../CSBP/CSBP.csproj --output ../THIRD-PARTY-NOTICES.TXT
cd ../Asciidoc
./asciidoc-csbp.sh
cd ../Install
# Browser cannot show html file in opt folder
cp -a ../Asciidoc/Csbp-Hilfe.html /opt/Haushalt/CSBP

dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net6.0 -r ubuntu-x64 --self-contained true
build "net6.0/ubuntu-x64" "csbp-net6-ubuntu-x64-runtime.zip"
dotnet publish ~/cs/csbp/CSBP/CSBP.csproj -c Release -f net6.0 -r win-x64 --self-contained true
build "net6.0/win-x64" "csbp-net6-win-x64-runtime.zip"

cp -a linux/#InstallUpdateCsbp.sh ./InstallUpdateCsbp.sh_.txt
cp -a windows/#InstallUpdateCsbp.cmd ./InstallUpdateCsbp.cmd_.txt
cp -a csbp/Csbp-Hilfe.html .

cat << EOT >ftp.txt
put InstallUpdateCsbp.sh_.txt
put InstallUpdateCsbp.cmd_.txt
put Csbp-Hilfe.html
put csbp-net6-ubuntu-x64-runtime.zip
put csbp-net6-win-x64-runtime.zip
EOT

# Upload via sftp
./#Upload.sh ftp.txt

rm -f ./ftp.txt
rm -f ./InstallUpdateCsbp.sh_.txt
rm -f ./InstallUpdateCsbp.cmd_.txt
rm -f ./Csbp-Hilfe.html
