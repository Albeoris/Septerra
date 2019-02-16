# Septerra
Septerra Core Tools

Starting without parameters will lead to the launch of the game from the current folder or the path form the windows registry.
<br/>
run [gameFolder] [gameArgs]<br/>
  Example: run . -M<br/>
  will start the game from the current directory with -M argument to disable movies.<br/>
<br/>
unpack [gameFolder] [outputFolder] [-convert] [-rename]<br/>
 Example: unpack . .\Data -convert<br/>
  will unpack resources from the current folder to the \Data folder, convert all known resources to the user-friendly formats and rename known files.<br/>
<br/>
convert type sourceFolder [sourceMask]<br/>
convert type sourceFile [targetFile]<br/>
  Types: tx2txt, am2tiff, tiff2am, vssf2mp3, mp32vssf<br/>
<br/>
  Example: convert am2tiff .\Data\anim *.am<br/>
  will convert all *.am files to TIFFs in the \Data\anim folder.<br/>
<br/>
  Example: convert tiff2am 08219990.tiff<br/>
  will convert 08219990.tiff to 08219990.am<br/>
<br/>
  Example: convert tx2txt 0500000B.tx "Azziz's Temple.txt"<br/>
  will convert 0500000B.tx to Azziz's Temple.txt<br/>
