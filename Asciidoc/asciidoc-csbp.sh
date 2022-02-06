# sudo apt-get install -y asciidoctor
# sudo gem update asciidoctor
# sudo gem install asciidoctor-pdf
asciidoctor -v -b html5 -o Csbp-Hilfe.html root/Csbp-Hilfe.adoc
# asciidoctor-pdf -v --trace -b pdf -o Csbp-Hilfe.pdf root/Csbp-Hilfe.adoc
