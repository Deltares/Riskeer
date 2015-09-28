<?xml version="1.0"?>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
    <xsl:output method="html"/>

	<xsl:variable name="fxcop.root" select="/FxCopReport"/>
	<xsl:variable name="fxcop.version" select="$fxcop.root/@Version" />
	<xsl:variable name="fxcop.lastAnalysis" select="$fxcop.root/@LastAnalysis"/>
	<xsl:variable name="message.list" select="$fxcop.root//Messages"/>

<!--
    <xsl:template match="/">
        <xsl:choose>
            <xsl:when test="cruisecontrol/FxCopReport">
                <xsl:apply-templates select="cruisecontrol/FxCopReport" />
            </xsl:when>
            <xsl:when test="cruisecontrol/build/FxCopReport">
                <xsl:apply-templates select="cruisecontrol/build/FxCopReport" />
            </xsl:when>
            <xsl:otherwise>
                <div class="header bgLight">
                    <xsl:text>FxCop Analysis Report</xsl:text>
                </div>
                <div style="padding-left: 3px;">
                    <xsl:text>FxCop results not available</xsl:text>
                </div>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template> -->

    <xsl:template match="/FxCopReport">
        <div id="fxcop" class="report">
            <xsl:variable name="Issues" select="count(.//Message)"></xsl:variable>
            <div class="header bgLight">
                <xsl:text>FxCop Version </xsl:text>
                <xsl:value-of select="@Version" />
                <xsl:text> Analysis Report (</xsl:text>
                <xsl:value-of select="$Issues" />
                <xsl:text> Issues)</xsl:text>
            </div>
            <div id="problemSummary">
               <strong><xsl:text>Summary</xsl:text></strong>
               <table class="fxcopSummary">
                    <tbody>
                        <tr>
                            <td>Assemblies tested:</td>
                            <td><xsl:value-of select="count(.//Module)"/></td>
                        </tr>
                        <tr>
                            <td>Assembly violations:</td>
                            <td><xsl:value-of select="count(.//Module/Messages/Message)"/></td>
                        </tr>
                        <tr>
                            <td>Resource violations:</td>
                            <td><xsl:value-of select="count(.//Resources//Message)"/></td>
                        </tr>
                        <tr>
                            <td>Type violations:</td>
                            <td><xsl:value-of select="count(.//Type/Messages/Message)"/></td>
                        </tr>
                        <tr>
                            <td>Member violations:</td>
                            <td><xsl:value-of select="count(.//Member//Message)"/></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
	</xsl:template>
</xsl:stylesheet>
