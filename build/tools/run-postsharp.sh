#!/bin/sh

# $1 - path to PostSharp
# $2 - dll name
# $3 - name of the *.snk file (optional)

if [ "$3" == "" ]
then
  export SNK=DelftTools.snk
else
  export SNK=$3
fi

if [ ! -d PostSharp ]
then
    mkdir ./PostSharp
fi

cd PostSharp


if [ -f $2 -a -f ../$2 ]
then


if [ "`stat -f %m $2`" == "`stat -f %m ../$2`" ]
then
    echo Skipping $2 ...
    exit
else
echo Time stamps for $2 are not equal: "`stat -f %m $2`" == "`stat -f %m ../$2`" 
fi

fi


cp $1/Default.psproj ./

if [ -f ../../../$SNK ]
then
    mono $1/PostSharp.exe Default.psproj ../$2 /P:ResolvedReferences= /P:SearchPath= /P:Output=./$2 /P:IntermediateDirectory=./ /P:CleanIntermediate=false /P:SignAssembly=true /P:PrivateKeyLocation=../../../$SNK
else
    mono $1/PostSharp.exe Default.psproj ../$2 /P:ResolvedReferences= /P:SearchPath= /P:Output=./$2 /P:IntermediateDirectory=./ /P:CleanIntermediate=false /P:SignAssembly=false /P:PrivateKeyLocation=.
fi

cp -p ./$2 ../
exit 0

