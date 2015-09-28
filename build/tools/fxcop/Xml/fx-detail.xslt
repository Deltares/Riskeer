<?xml version="1.0" encoding="UTF-8"?>
<!-- This XSL converts FxCop output to a detailed view merge compliant -->
<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>  
	<xsl:output method="xml" version="1.0" indent="yes"/>
	<xsl:variable name="fxcop.root" select="//FxCopReport"/>
	<!-- Default match pattern -->
	<xsl:template match="/">
		<xsl:element name="detail">
			<xsl:apply-templates select="$fxcop.root"/>
		</xsl:element>
	</xsl:template>
	<!-- Necessary for enumeration of xml elements -->	
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
	<!-- Binary by binary summation of errors -->
	<xsl:template match="Module">
		<xsl:variable name="All" select="count(*//Issue)" />
		<!-- Wrap in element Binary name-->
		<xsl:element name="Assembly">
			<xsl:attribute name="id"><xsl:value-of select="@Name"/></xsl:attribute>
			<xsl:element name="total"><xsl:value-of select="$All" /></xsl:element>
			<!-- variables needed for computation -->
			<xsl:variable name="CriticalErrors" select="count(*//Issue[@Level='CriticalError'])" />
			<xsl:variable name="Errors" select="count(*//Issue[@Level='Error'])" />
			<xsl:variable name="CriticalWarnings" select="count(*//Issue[@Level='CriticalWarning'])" />
			<xsl:variable name="Warnings" select="count(*//Issue[@Level='Warning'])" />
			<!-- results -->
			<CriticalErrors><xsl:value-of select="$CriticalErrors"/></CriticalErrors><xsl:text>&#13;</xsl:text>
			<Errors><xsl:value-of select="$Errors"/></Errors><xsl:text>&#13;</xsl:text>
			<CriticalWarnings><xsl:value-of select="$CriticalWarnings"/></CriticalWarnings><xsl:text>&#13;</xsl:text>
			<Warnings><xsl:value-of select="$Warnings"/></Warnings><xsl:text>&#13;</xsl:text>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>