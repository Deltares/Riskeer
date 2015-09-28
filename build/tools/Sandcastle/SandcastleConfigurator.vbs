' This script replaces some hard-coded paths in the sandcastle.config file
' Modified by Souhad F. Alwan to work with Sandcastle October 2007 CTP
Main()

Public Sub Main()
   Dim xmlDoc
   Dim fso
   Dim inFile, outFile, outputPath
   Dim nodeType, sandcastlePath, sandcastleExamplePath
   Dim sharedContentPath, refContentPath, fbContentPath
   Dim xmlSourceNode

   Set fso = CreateObject("Scripting.FileSystemObject")
   If WScript.Arguments.Named.Exists("?") Then
       ShowUsage
       WScript.Quit(0)
   End If

   ' Grab arguments    
   nodeType = WScript.Arguments.Named("nodeType")
   sandcastlePath = WScript.Arguments.Named("path")
   sharedContentPath = WScript.Arguments.Named("shared")
   refContentPath = WScript.Arguments.Named("ref")
   fbContentPath = WScript.Arguments.Named("feedback")
   inFile = WScript.Arguments.Named("in")
   outFile = WScript.Arguments.Named("out")
   outputPath = WScript.Arguments.Named("output")

   ' Check arguments
   If Not fso.FileExists(inFile) Then
       WScript.StdErr.WriteLine "ERROR: InFile '" + inFile + "' not found."
       ShowUsage
       WScript.Quit(1)
   End If

   ' Set defaults
   If nodeType = "" Then
       nodeType = "comments"
   End If

   If sandcastlePath = "" Then
       sandcastlePath = "C:\Program Files\Sandcastle"
   End If

   If sharedContentPath = "" Then
       sharedContentPath = sandcastlePath + "\Presentation\Prototype\content\shared_content.xml"
   End If

   If refContentPath = "" Then
       refContentPath = sandcastlePath + "\Presentation\Prototype\content\reference_content.xml"
   End If
   ' fbContentPath 
   If fbContentPath = "" Then
       fbContentPath = sandcastlePath + "\Presentation\Prototype\content\feedback_content.xml"
   End If
   sandcastleExamplePath = sandcastlePath + "\Examples"

   ' Load original sandcastle.config file    

   Set xmlDoc = CreateObject("MSXML2.DOMDocument")
   xmlDoc.preserveWhiteSpace = true
   xmlDoc.load inFile

   ' Remove all current content paths
   Set xmlNodes = xmlDoc.selectNodes("//component[@type='Microsoft.Ddue.Tools.SharedContentComponent']/content")
   For Each xmlNode in xmlNodes
       xmlNode.parentNode.removeChild xmlNode
   Next

   ' Replace them with new ones
   Set xmlSourceNode = xmlDoc.selectSingleNode("//component[@type='Microsoft.Ddue.Tools.SharedContentComponent']")

   Dim contentNode
     ' here put feedback reference
   Set contentNode = xmlDoc.createElement("content")
   contentNode.setAttribute "file", fbContentPath
   xmlSourceNode.appendChild(contentNode)
   
   Set contentNode = xmlDoc.createElement("content")
   contentNode.setAttribute "file", sharedContentPath
   xmlSourceNode.appendChild contentNode
   Set contentNode = xmlDoc.createElement("content")
   contentNode.setAttribute "file", refContentPath
   xmlSourceNode.appendChild(contentNode)
 

   ' Remove save output path node from element so we can add our replacement
   Set xmlNodes = xmlDoc.selectNodes("//component[@type='Microsoft.Ddue.Tools.SaveComponent']/save")
   For Each xmlNode in xmlNodes
       xmlNode.parentNode.removeChild xmlNode
   Next

   ' Replace the save path with the new one
   Set xmlSourceNode = xmlDoc.selectSingleNode("//component[@type='Microsoft.Ddue.Tools.SaveComponent']")
   Set contentNode = xmlDoc.createElement("save")
   contentNode.setAttribute "base", outputPath + "\html"
   contentNode.setAttribute "path", "concat(/html/head/meta[@name='guid']/@content,'.htm')"
   contentNode.setAttribute "indent", "false"
   contentNode.setAttribute "omit-xml-declaration", "true"
   xmlSourceNode.appendChild(contentNode)
   
    ' Replace the save path with the new one 
   Set xmlSourceNode = xmlDoc.selectSingleNode("//component[@type='Microsoft.Ddue.Tools.CopyFromIndexComponent']/index")
   Set contentNode = xmlDoc.createElement("data")
   contentNode.setAttribute "files", outputPath + "\reflection.xml"
   xmlSourceNode.appendChild(contentNode)

  ' Replace the save path with the new one 
   Set xmlSourceNode = xmlDoc.selectSingleNode("//component[@type='Microsoft.Ddue.Tools.ResolveReferenceLinksComponent2']")
   Set contentNode = xmlDoc.createElement("targets")
   contentNode.setAttribute "files", outputPath + "\reflection.xml"
   contentNode.setAttribute "type", "local"
   xmlSourceNode.appendChild(contentNode)
   ' Find the parent node for the new data elements
   Set xmlSourceNode = xmlDoc.selectSingleNode("//index[@name='" + nodeType + "']")
   For Each arg in WScript.Arguments.Unnamed
       ' Add new data node
       Dim dataNode
       Set dataNode = xmlDoc.createElement("data")
       dataNode.setAttribute "files", arg
       ' dataNode.setAttribute "files", "lablab"
       xmlSourceNode.appendChild(dataNode)
   Next
   If outFile = "" Then
       ' If no outfile was given, StdOut is used    
       WScript.StdOut.Write xmlDoc.xml
   Else
       ' Otherwise, save the file
       xmlDoc.save outFile
   End If
End Sub

Public Sub ShowUsage()
   WScript.StdErr.WriteLine "This script replaces some hard-coded paths in the sandcastle.config file."
   WScript.StdErr.WriteLine "CScript.exe SandcastleConfigurator.vbs /in:""original config"" [/out:""new config""] [/output:""help output path"" [/path:""Sandcastle path""] ""Xml file 1"" ""Xml file 2"" ..."
End Sub