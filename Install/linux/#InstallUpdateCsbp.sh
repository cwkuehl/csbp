#! /bin/bash
SCRIPT="${BASH_SOURCE[0]}"
if [ $SCRIPT = "./#InstallUpdateCsbp.sh" ]
then
  cp -a $SCRIPT _.sh
  exec ./_.sh
  # exit
fi

echo "Installation and update for program CSBP (c) 2022 cwkuehl.de"

function download {
  curl -o ./temp/csbp.zip 'https://cwkuehl.de/wp-content/uploads/2022/01/csbp-net5-ubuntu-x64.zip'
  rm -rf temp/zip
  unzip ./temp/csbp.zip -d ./temp/zip
  cp -rf ./temp/zip/csbp/* .
  rm -rf temp/zip
  rm ./temp/csbp.zip
}

if test ! -d temp
then
  mkdir temp
fi

if test -e Jhh-1.0.jar
then
  download
  rm ./Empty*.*
else
  download
fi

# if test ! -e log4j.properties
# then
#   cp Leerlog4j.properties log4j.properties
# fi

# if test ! -e ServerConfig.properties
# then
#   cp LeerServerConfig.properties ServerConfig.properties
# fi

if test ! -d ~/csbp
then
  mkdir ~/csbp
fi

# if test ! -d ~/hsqldb/JHH6.properties
# then
#   cp LeerJHH6.properties ~/hsqldb/JHH6.properties
#   cp LeerJHH6.script ~/hsqldb/JHH6.script
# fi

chmod +x ./#Csbp.sh

# Start CSBP
(./#Csbp.sh)&

rm ./_.sh
