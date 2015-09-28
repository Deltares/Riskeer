<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:fo="http://www.w3.org/1999/XSL/Format" 
                xmlns:xs="http://www.w3.org/5001/XMLSchema" 
                xmlns:fn="http://www.w3.org/5005/xpath-functions" 
                xmlns="http://www.w3.org/TR/xhtml1/strict" 
                xmlns:exsl="http://exslt.org/common" 
                extension-element-prefixes="exsl">

  <xsl:param name="SolutionPath" />
  <xsl:param name="ReflectionXml" />
  <xsl:param name="Server" />
                  
                  
   <xsl:template match="/topics">
    <html>
      <head>
        <title>List of Code Elements missing XML Comments</title>
      </head>
      <style>
        #Module {font-family: Verdana; font-size: 10pt; color: black; font-weight: bold}
        #Text {font-family: Verdana; font-size: 8pt; color: black}
        #Title {font-family: Verdana; font-size: 14pt; color: black; font-weight: bold}
        .ColumnHeader {font-family: Verdana; font-size: 8pt; background-color:white; color: black}
        .CriticalError {font-family: Verdana; font-size: 8pt; color: darkred; font-weight: bold; vertical-align: middle; }
        .Error {font-family: Verdana; font-size: 8pt; color: royalblue; font-weight: bold; vertical-align: middle; }
        .CriticalWarning {font-family: Verdana; font-size: 8pt; color: darkorange; font-weight: bold; vertical-align: middle; }
        .Warning {font-family: Verdana; font-size: 8pt; color: darkgray; font-weight: bold; vertical-align: middle; }
        .Information {font-family: Verdana; font-size: 8pt; color: black; font-weight: bold; vertical-align: middle; }

        .PropertyName {font-family: Verdana; font-size: 8pt; color: black; font-weight: bold}
        .PropertyContent {font-family: Verdana; font-size: 8pt; color: black}
        .NodeIcon { font-family: WebDings; font-size: 12pt; color: navy; padding-right: 5;}
        .MessagesIcon { font-family: WebDings; font-size: 12pt; color: red;}
        .RuleDetails { padding-top: 10;}
        .SourceCode { background-color:#DDDDFF; }
        .RuleBlock { background-color:#EEEEFF; }
        .MessageNumber { font-family: Verdana; font-size: 10pt; color: darkred; }
        .MessageBlock { font-family: Verdana; font-size: 10pt; color: darkred; }
        .Resolution {font-family: Verdana; font-size: 8pt; color: black; }
        .NodeLine { font-family: Verdana; font-size: 9pt;}
        .Note { font-family: Verdana; font-size: 9pt; color:black; background-color: #DDDDFF; }
        .NoteUser { font-family: Verdana; font-size: 9pt; font-weight: bold; }
        .NoteTime { font-family: Verdana; font-size: 8pt; font-style: italic; }
        .NoteJustification { font-family: Verdana; font-size: 8pt; font-style: italic; }
        .Button { font-family: Verdana; font-size: 9pt; color: blue; background-color: #EEEEEE; border-style: outset;}
        a:link { color: blue; text-decoration: none; }
        a:visited { color: blue; text-decoration: none; }
        a:active { color: blue; text-decoration: none; }
      </style>
      <script>
        function ToggleState(blockId)
        {
        var block = document.getElementById(blockId);

        if (block != null) {
        if (block.style.display=='none')
        {
        block.style.display='block';

        if (block.className == 'MessageDiv')
        {
        var toggle = document.getElementById(blockId + "Toggle");
        toggle.innerHTML = "6";
        }
        }
        else
        {
        block.style.display='none';

        if (block.className=='MessageDiv')
        {
        var toggle = document.getElementById(blockId + "Toggle");
        toggle.innerHTML = "4";
        }
        }
        }
        }


        function ToggleNodeState(blockId, status)
        {

        var nodes = document.getElementsByName(blockId);
        for (i = 0; i != nodes.length;i++)
        {
        var block = nodes[i];
        if (block != null)
        {

        if (block.getAttribute('status') == status)
        {
        if (block.style.display=='none')
        {
        block.style.display='block';
        }
        else
        {
        block.style.display='none';
        }
        }
        }
        }
        }


        function trace(node) {
        alert(node.className + " " + node.getAttribute('status'));
        }

        function SwitchAll(how, status)
        {
        var nodes = document.getElementsByTagName("div");
        for (i = 0; i != nodes.length;i++)
        {
        var block = nodes[i];
        if (block != null)
        {
        if (IsBlockWithMatchingStatus('NodeDiv', block, status)
        || IsBlockWithMatchingStatus('MessageBlockDiv', block, status)
        || IsMessageDivWithActionNone(block, how)
        )
        {
        block.style.display=how;
        }
        }
        }
        }


        function IsBlockWithMatchingStatus(className, block, status)
        {
        if (block.className != className) return false;

        if (block.getAttribute('status') == status)
        return true;
        else
        return false;

        }

        function IsMessageDivWithActionNone(block, how)
        {
        if (block.className != 'MessageDiv') return false;
        if (how != 'none') return false;

        //as we're collapsing the tree, set the correct toggle icon
        var toggle = document.getElementById(block.id + "Toggle");
        toggle.innerHTML = "4";

        return true;
        }

        function ExpandAll(status)
        {
        SwitchAll('block', status);
        }

        function CollapseAll(status)
        {
        SwitchAll('none', status);
        }

        function DoNothing() {}

        function DoNothing2() {}

        function DoNothing3() {}

        function ButtonState(blockId)
        {
        var block = document.getElementById(blockId);
        if (block.style.borderStyle=='inset')
        {
        block.style.borderStyle='outset';
        }
        else
        {
        block.style.borderStyle='inset';
        }
        }
      </script>
      <body>
        <xsl:value-of select="$ReflectionXml"/>
        <br />
        <xsl:value-of select="$SolutionPath"/>
        <br />
        <!-- test-->
        <xsl:variable name="SolPathNodeSet" select="exsl:node-set($SolutionPath)"/>
        <xsl:variable name="SolPathString" select="$SolutionPath/D:/projects"/>
        <xsl:value-of select="$SolPathString"/>
        <!--xsl:variable name="PathToPass">
        <xsl:choose>
          <xsl:when test="$Server = 'Yes'">D:\Projects\DelftTools\build</xsl:when>
          <xsl:when test="$Server = 'No'">D:\TeamCity\d99fce49b74fe308\build</xsl:when>
        </xsl:choose>
        </xsl:variable-->
        <!--xsl:value-of select="$(PathToPass)"/-->
        <!--xsl:for-each select="topic[starts-with(@id,'N:')]">
          <br/>
          <div id="Module">
            [<xsl:value-of select="position()"/>]  <xsl:value-of select="substring-after(@id,'N:')"/> Namespace
          </div>
          <br/>
          <xsl:call-template name="Namespace">
            <xsl:with-param name="topic" select="."/>
          </xsl:call-template>
        </xsl:for-each-->
        <br />
        <!--xsl:for-each select="topic[starts-with(@id,'N:')]">
          <div id="Module">
            [<xsl:value-of select="position()"/>]	<xsl:value-of select="substring-after(@id,'N:')"/> Namespace
          </div>
          <br />
          <xsl:call-template name="MissedComment">
            <xsl:with-param name="topic" select="."/>
            <xsl:with-param name="path" select="$(PathToPass)"/>
          </xsl:call-template>
          <br />
        </xsl:for-each-->

      </body>
    </html>
  </xsl:template>

  <xsl:template name="MissedComment">
    <xsl:param name="topic"/>
    <xsl:param name="path"/>
    <!-- Give list of all code items in this namespace -->
    <xsl:variable name="namespace" select="substring-after(@id,'N:')"/>
    <xsl:variable name="NsId" select="@id"/>
    <xsl:variable name="assembly" select="$ReflectionXml/reflection/apis/api/containers[namespace/@api = $NsId]"/>
    <xsl:variable name="assemblyName" select="exsl:node-set($assembly)/library[1]/@assembly"/>
    <xsl:variable name="ClassInterface" select="/topics/topic[starts-with(@id,concat('T:',$namespace,'.'))]"/>
    <xsl:variable name="Method" select="/topics/topic[starts-with(@id,concat('M:',$namespace,'.'))]"/>
    <xsl:variable name="OverloeadedMethod" select="/topics/topic[starts-with(@id,concat('Overload:',$namespace,'.'))]"/>
    <xsl:variable name="Property" select="/topics/topic[starts-with(@id,concat('P:',$namespace,'.'))]"/>
    <xsl:variable name="Event" select="/topics/topic[starts-with(@id,concat('E:',$namespace,'.'))]"/>
    <xsl:variable name="Field" select="/topics/topic[starts-with(@id,concat('F:',$namespace,'.'))]"/>
    <xsl:variable name="ClassInterfaceCount" select="count($ClassInterface)"/>
    <xsl:variable name="MethodCount" select="count($Method)"/>
    <xsl:variable name="OverloeadedMethodCount" select="count($OverloeadedMethod)"/>
    <xsl:variable name="PropertyCount" select="count($Property)"/>
    <xsl:variable name="EventCount" select="count($Event)"/>
    <!-- get list of all code elements not having comments -->
    <xsl:variable name="FieldCount" select="count($Field)"/>
    <xsl:value-of select="count(exsl:node-set($path))"/>

  
    <xsl:variable name="namespaceFile" select="document($fullpath)"/>
    <xsl:if test="$ClassInterfaceCount > 0">
      <div class="NodeLine">
        <xsl:attribute name="onClick">
          javascript:ToggleNodeState('<xsl:value-of select="concat($namespace,'Classes')"/>','<xsl:value-of select="'Active'"/>');
        </xsl:attribute>
        <a href="javascript:DoNothing()">
          <nobr style="color: navy;">{} </nobr>
        </a>
        Classes / Interfaces missing Xml Comments
      </div>
      <div class="NodeDiv" style="display: none; position: relative; padding-left: 11;" status="Active">
        <xsl:attribute name="id">
          <xsl:value-of select="concat($namespace,'Classes')"/>
        </xsl:attribute>
        <table id="Text">
          <tr>
            <th>Class Name</th>
          </tr>
          <xsl:for-each select="$ClassInterface">
            <xsl:variable name="id" select="@id"/>

            <xsl:if test="not(boolean(exsl:node-set($namespaceFile)/doc/members/member[@name=$id]/summary))">
              <tr>
                <td>
                  <xsl:value-of select="substring-after(substring-after($id,$namespace),'.')"/>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </table>
      </div>
    </xsl:if>
    <!-- Search for methods -->
    <!--xsl:if test="$MethodCount > 0">
      <div class="NodeLine">
        <xsl:attribute name="onClick">
          javascript:ToggleNodeState('<xsl:value-of select="concat($namespace,'Methods')"/>','<xsl:value-of select="'Active'"/>');
        </xsl:attribute>
        <a href="javascript:DoNothing()">
          <nobr style="color: navy;">{} </nobr>
        </a>
        Methods missing Xml Comments
      </div>
      <div class="NodeDiv" style="display: none; position: relative; padding-left: 11;" status="Active">
        <xsl:attribute name="id">
          <xsl:value-of select="concat($namespace,'Methods')"/>
        </xsl:attribute>
        <table id="Text">
          <tr>
            <th>Method Name</th>
          </tr>
          <xsl:for-each select="$Method">
            <xsl:variable name="id" select="@id"/>
            <xsl:if test="not(boolean($namespaceFile/doc/members/member[@name=$id]/summary))">
              <tr>
                <td>
                  <xsl:value-of select="substring-after(substring-after($id,$namespace),'.')"/>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </table>
      </div>
    </xsl:if-->
    <!-- Search for properties -->
    <!--xsl:if test="$PropertyCount > 0">
      <div class="NodeLine">
        <xsl:attribute name="onClick">
          javascript:ToggleNodeState('<xsl:value-of select="concat($namespace,'Props')"/>','<xsl:value-of select="'Active'"/>');
        </xsl:attribute>
        <a href="javascript:DoNothing()">
          <nobr style="color: navy;">{} </nobr>
        </a>
        Properties missing Xml Comments
      </div>
      <div class="NodeDiv" style="display: none; position: relative; padding-left: 11;" status="Active">
        <xsl:attribute name="id">
          <xsl:value-of select="concat($namespace,'Props')"/>
        </xsl:attribute>
        <table id="Text">
          <tr>
            <th>Property Name</th>
          </tr>
          <xsl:for-each select="$Property">
            <xsl:variable name="id" select="@id"/>
            <xsl:if test="not(boolean($namespaceFile/doc/members/member[@name=$id]/summary))">
              <tr>
                <td>
                  <xsl:value-of select="substring-after(substring-after($id,$namespace),'.')"/>
                </td>
              </tr>

            </xsl:if>
          </xsl:for-each>
        </table>
      </div>
    </xsl:if-->
    <!-- Search for fields -->
    <!--xsl:if test="FieldCount > 0">
      <div class="NodeLine">
        <xsl:attribute name="onClick">
          javascript:ToggleNodeState('<xsl:value-of select="concat($namespace,'Fields')"/>','<xsl:value-of select="'Active'"/>');
        </xsl:attribute>
        <a href="javascript:DoNothing()">
          <nobr style="color: navy;">{} </nobr>
        </a>
        Fields missing Xml Comments
      </div>
      <div class="NodeDiv" style="display: none; position: relative; padding-left: 11;" status="Active">
        <xsl:attribute name="id">
          <xsl:value-of select="concat($namespace,'Fields')"/>
        </xsl:attribute>
        <table id="Text">
          <tr>
            <th>Field Name</th>
          </tr>
          <xsl:for-each select="$Field">
            <xsl:variable name="id" select="@id"/>
            <xsl:if test="not(boolean($namespaceFile/doc/members/member[@name=$id]/summary))">
              <tr>
                <td>
                  <xsl:value-of select="substring-after(substring-after($id,$namespace),'.')"/>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </table>
      </div>
    </xsl:if-->
    <!-- Search for events -->
    <!--xsl:if test="EventCount > 0">
      <div class="NodeLine">
        <xsl:attribute name="onClick">
          javascript:ToggleNodeState('<xsl:value-of select="concat($namespace,'Events')"/>','<xsl:value-of select="'Active'"/>');
        </xsl:attribute>
        <a href="javascript:DoNothing()">
          <nobr style="color: navy;">{} </nobr>
        </a>
        Events missing Xml Comments
      </div>
      <div class="NodeDiv" style="display: none; position: relative; padding-left: 11;" status="Active">
        <xsl:attribute name="id">
          <xsl:value-of select="concat($namespace,'Events')"/>
        </xsl:attribute>
        <table id="Text">
          <tr>
            <th>Event Name</th>
          </tr>
          <xsl:for-each select="$Event">
            <xsl:variable name="id" select="@id"/>
            <xsl:if test="not(boolean($namespaceFile/doc/members/member[@name=$id]/summary))">
              <tr>
                <td>
                  <xsl:value-of select="substring-after(substring-after($id,$namespace),'.')"/>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </table>
      </div>
    </xsl:if-->
  </xsl:template>
  <xsl:template name="CodeItem">
    <xsl:param name="Count"/>
    <xsl:param name="namespace"/>
    <xsl:param name="Item"/>
    <xsl:param name="ItemName"/>
    <!--xsl:param name="namespaceFile" /-->
    <xsl:if test="$Count > 0">
      <div class="NodeLine">
        <xsl:attribute name="onClick">
          javascript:ToggleNodeState('<xsl:value-of select="concat($namespace,$ItemName)"/>','<xsl:value-of select="'Active'"/>');
        </xsl:attribute>
        <a href="javascript:DoNothing()">
          <nobr style="color: navy;">{} </nobr>
        </a>
        <xsl:value-of select="$ItemName"/> missing Xml Comments
      </div>
      <div class="NodeDiv" style="display: none; position: relative; padding-left: 11;" status="Active">
        List: <br/>
        <xsl:attribute name="id">
          <!--xsl:value-of select="concat($namespace,$ItemName)"/-->
          <xsl:value-of select="$namespace"/>
        </xsl:attribute>
        <xsl:for-each select="$Item">
          <xsl:variable name="id" select="@id"/>
          <xsl:choose>
            <xsl:when test="boolean($namespaceFile/doc/members/member[@name=$id]/summary)"/>
            <xsl:otherwise>
              <xsl:value-of select="substring-after(substring-after($id,$namespace),'.')"/>
              <br/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </div>
    </xsl:if>
  </xsl:template>
  <xsl:template name="Namespace">
    <xsl:param name="topic"/>
    <xsl:variable name="namespace" select="substring-after(@id,'N:')"/>
    <xsl:variable name="NsId" select="@id"/>
    <xsl:variable name="assembly" select="$ReflectionXml/reflection/apis/api/containers[namespace/@api = $NsId]"/>
    <xsl:variable name="assemblyName" select="exsl:node-set($assembly)/library[1]/@assembly"/>
    <!--xsl:value-of select="$test[0]" /-->
    <xsl:variable name="ClassInterface" select="/topics/topic[starts-with(@id,concat('T:',$namespace,'.'))]"/>
    <xsl:variable name="Method" select="/topics/topic[starts-with(@id,concat('M:',$namespace,'.'))]"/>
    <xsl:variable name="OverloeadedMethod" select="/topics/topic[starts-with(@id,concat('Overload:',$namespace,'.'))]"/>
    <xsl:variable name="Property" select="/topics/topic[starts-with(@id,concat('P:',$namespace,'.'))]"/>
    <xsl:variable name="Event" select="/topics/topic[starts-with(@id,concat('E:',$namespace,'.'))]"/>
    <xsl:variable name="Field" select="/topics/topic[starts-with(@id,concat('F:',$namespace,'.'))]"/>
    <xsl:variable name="ClassInterfaceCount" select="count($ClassInterface)"/>
    <xsl:variable name="MethodCount" select="count($Method)"/>
    <xsl:variable name="OverloeadedMethodCount" select="count($OverloeadedMethod)"/>
    <xsl:variable name="PropertyCount" select="count($Property)"/>
    <xsl:variable name="EventCount" select="count($Event)"/>
    <!-- get list of all code elements not having comments -->
    <xsl:variable name="FieldCount" select="count($Field)"/>
      <xsl:if test="$ClassInterfaceCount > 0">
      <table id="Text" cellspacing="0" cellpadding="3">
        <tr>
          <td width="150">
            <img src="C:\Documents and Settings\alwan\My Documents\Altova Projects\Missed Comments\pubclass.gif"/>Public Classes
          </td>
          <td bgcolor="#9900FF">
            <xsl:attribute name="width">
              <xsl:value-of select="$ClassInterfaceCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:attribute name="width">
              <xsl:value-of select="500 - $ClassInterfaceCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:value-of select="$ClassInterfaceCount"/>
          </td>
        </tr>
      </table>
    </xsl:if>
    <xsl:if test="$MethodCount > 0">
      <table id="Text" cellspacing="0" cellpadding="3">
        <tr>
          <td width="150">
            <img src="C:\Documents and Settings\alwan\My Documents\Altova Projects\Missed Comments\pubmethod.gif"/>Public Methods
          </td>
          <td bgcolor="#6600FF">
            <xsl:attribute name="width">
              <xsl:value-of select="$MethodCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:attribute name="width">
              <xsl:value-of select="500 - $MethodCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:value-of select="$MethodCount"/>
          </td>
        </tr>
      </table>
    </xsl:if>
    <xsl:if test="$PropertyCount > 0">
      <table id="Text" cellspacing="0" cellpadding="3">
        <tr>
          <td width="150">
            <img src="C:\Documents and Settings\alwan\My Documents\Altova Projects\Missed Comments\pubproperty.gif"/>Public Properties
          </td>
          <td bgcolor="#0099FF">
            <xsl:attribute name="width">
              <xsl:value-of select="$PropertyCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:attribute name="width">
              <xsl:value-of select="500 - $PropertyCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:value-of select="$PropertyCount"/>
          </td>
        </tr>
      </table>
    </xsl:if>
    <xsl:if test="$EventCount > 0">
      <table id="Text" cellspacing="0" cellpadding="3">
        <tr>
          <td width="150">
            <img src="C:\Documents and Settings\alwan\My Documents\Altova Projects\Missed Comments\pubevent.gif"/>Public Events
          </td>
          <td bgcolor="#FFFF00">
            <xsl:attribute name="width">
              <xsl:value-of select="$EventCount"/>
            </xsl:attribute>
          </td>
          <xsl:if test="$EventCount = 0">
            <td/>
          </xsl:if>
          <td>
            <xsl:attribute name="width">
              <xsl:value-of select="500 - $EventCount"/>
            </xsl:attribute>
          </td>
          <td>
            <xsl:value-of select="$EventCount"/>
          </td>
        </tr>
      </table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
