#! /bin/bash
SCRIPT="${BASH_SOURCE[0]}"
if [[ $SCRIPT = "./#InstallUpdateCsbp.sh" || $SCRIPT = "$PWD/#InstallUpdateCsbp.sh" ]]; then
  cp -a $SCRIPT _.sh
  exec ./_.sh
  # exit
fi

function download {
  curl -o ./temp/csbp.zip 'https://cwkuehl.de/wp-content/uploads/2022/01/csbp-net6-ubuntu-x64-runtime.zip'
  rm -rf temp/zip
  unzip ./temp/csbp.zip -d ./temp/zip
  cp -rf ./temp/zip/csbp/* .
  rm -rf temp/zip
  rm ./temp/csbp.zip
}

CURRENTDATE=`date +"%Y-%m-%d %T"`
echo "$CURRENTDATE Installation and update for program CSBP (c) 2022 cwkuehl.de" >> ./Log.txt

if [[ ! -d temp ]]; then
  mkdir temp
fi

download

DBNAME=""
if [[ -e ~/csbp/csbp.db || -e ~/hsqldb/csbp.db ]]; then
  echo "$CURRENTDATE Database exists." >> ./Log.txt
  rm ./EmptyCsbp.db
else
  echo "$CURRENTDATE Database initializing." >> ./Log.txt
  if [[ ! -d ~/csbp ]]; then
    mkdir ~/csbp
  fi
  DBNAME="$HOME/csbp/csbp.db"
  mv -n ./EmptyCsbp.db ~/csbp/csbp.db
  rm ./EmptyCsbp.db
fi

# Generate start script and start CSBP
echo "#! /bin/bash" > ./#Csbp0.sh
echo "# Start program CSBP (c) 2022 cwkuehl.de" >> ./#Csbp0.sh
echo "cd $PWD/publish" >> ./#Csbp0.sh
if [[ -z "$DBNAME" ]]; then
  echo "dotnet CSBP.dll" >> ./#Csbp0.sh
else
  echo "dotnet CSBP.dll \"DB_DRIVER_CONNECT=Data Source=$DBNAME\"" >> ./#Csbp0.sh
fi
chmod +x ./#Csbp0.sh
if [[ ! -e ./#Csbp.sh ]]; then
  mv ./#Csbp0.sh ./#Csbp.sh
fi
(./#Csbp.sh)&

rm ./_.sh
