<?xml version="1.0" encoding="utf-8"?>
<!-- This XSL converts FxCop output to a total overview -->
<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>  
	<xsl:output method="xml" version="1.0" indent="yes"/>
	<xsl:variable name="fxcop.root" select="//FxCopReport"/>
	<!-- Default match pattern -->
	<xsl:template match="/">
	<xsl:element name="overall">
		<CriticalErrors><xsl:value-of select="count(//Issue[@Level='CriticalError'])" /></CriticalErrors><xsl:text>&#13;</xsl:text>
		<Errors><xsl:value-of select="count(//Issue[@Level='Error'])" /></Errors><xsl:text>&#13;</xsl:text>
		<CriticalWarnings><xsl:value-of select="count(//Issue[@Level='CriticalWarning'])" /></CriticalWarnings><xsl:text>&#13;</xsl:text>
		<Warnings><xsl:value-of select="count(//Issue[@Level='Warning'])" /></Warnings><xsl:text>&#13;</xsl:text>
	</xsl:element>
	<!-- Necessary for enumeration of xml elements -->
	<xsl:apply-templates select="$fxcop.root"/>
	</xsl:template>
	<xsl:template match="FxCopReport">
		<xsl:apply-templates select="Targets" />
	</xsl:template>
	<xsl:template match="Targets">
		<xsl:apply-templates select="Target" />
	</xsl:template>
	<xsl:template match="Target">
		<xsl:apply-templates select="Modules" />
	</xsl:template>
	<xsl:template match="Modules">
		<xsl:apply-templates select="Module" />
	</xsl:template>
	<xsl:variable name="CriticalErrors" select="number(0)" />
	<xsl:template match="Module">
		<xsl:variable name="All" select="count(*//Issue)" />
	</xsl:template>
</xsl:stylesheet>