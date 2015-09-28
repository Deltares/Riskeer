<?xml version="1.0"?>
<!-- Copyright 2007 Kamstrup A/S -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
	<xsl:output method="html"/>
	<xsl:variable name="fxcop.root" select="//FxCopReport"/>

	<xsl:template match="/">
		<html>
			<head><title>Analysis Graph</title></head>
			<style>
				#Title {font-family: Verdana; font-size: 14pt; color: black; font-weight: bold}
				#Module {font-family: Verdana; font-size: 10pt; color: black; font-weight: bold}
				#Text {font-family: Verdana; font-size: 8pt; color: black}
        	</style>
			<body>
				<div id="Title">FxCop <xsl:value-of select="@Version"/> Analysis Graph</div><br />
				<div id="Module">Summary</div><br />
				<table cellpadding="3" cellspacing="3" id="Module">
					<tr>
						<td><img src="/img/fxcop/fxcop-critical-error.gif" /> Critical Errors</td>
						<td><xsl:value-of select="count(//Issue[@Level='CriticalError'])" /></td>
					</tr>
					<tr>
						<td><img src="/img/fxcop/fxcop-error.gif" /> Errors</td>
						<td><xsl:value-of select="count(//Issue[@Level='Error'])" /></td>
					</tr>
					<tr>
						<td><img src="/img/fxcop/fxcop-critical-warning.gif" /> Critical Warnings</td>
						<td><xsl:value-of select="count(//Issue[@Level='CriticalWarning'])" /></td>
					</tr>
					<tr>
						<td><img src="/img/fxcop/fxcop-warning.gif" /> Warnings</td>
						<td><xsl:value-of select="count(//Issue[@Level='Warning'])" /></td>
					</tr>
				</table><br />
				<xsl:apply-templates select="$fxcop.root"/>
			</body>
		</html>
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
		<div id="Module"><xsl:value-of select="@Name" /> (<xsl:value-of select="$All" />)</div>
		<xsl:variable name="CriticalErrors" select="count(*//Issue[@Level='CriticalError'])" />
		<xsl:variable name="Errors" select="count(*//Issue[@Level='Error'])" />
		<xsl:variable name="CriticalWarnings" select="count(*//Issue[@Level='CriticalWarning'])" />
		<xsl:variable name="Warnings" select="count(*//Issue[@Level='Warning'])" />
		<table id="Text" cellspacing="0" cellpadding="3">
			<tr>
				<td width="150"><img src="/img/fxcop/fxcop-critical-error.gif" /> Critical Errors</td>
				<td bgcolor="#990000">
					<xsl:attribute name="width"><xsl:value-of select="$CriticalErrors" /></xsl:attribute>
				</td>
				<td>
					<xsl:attribute name="width"><xsl:value-of select="500 - $CriticalErrors" /></xsl:attribute>
				</td>
				<td><xsl:value-of select="$CriticalErrors" /></td>
			</tr>
		</table>
		<table id="Text" cellspacing="0" cellpadding="3">
			<tr>
				<td width="150"><img src="/img/fxcop/fxcop-error.gif" /> Errors</td>
				<td bgcolor="#FF0000">
					<xsl:attribute name="width"><xsl:value-of select="$Errors" /></xsl:attribute>
				</td>
				<td>
					<xsl:attribute name="width"><xsl:value-of select="500 - $Errors" /></xsl:attribute>
				</td>
				<td><xsl:value-of select="$Errors" /></td>
			</tr>
		</table>
		<table id="Text" cellspacing="0" cellpadding="3">
			<tr>
				<td width="150"><img src="/img/fxcop/fxcop-critical-warning.gif" /> Critical Warnings</td>
				<td bgcolor="#FF9900">
					<xsl:attribute name="width"><xsl:value-of select="$CriticalWarnings" /></xsl:attribute>
				</td>
				<td>
					<xsl:attribute name="width"><xsl:value-of select="500 - $CriticalWarnings" /></xsl:attribute>
				</td>
				<td><xsl:value-of select="$CriticalWarnings" /></td>
			</tr>
		</table>
		<table id="Text" cellspacing="0" cellpadding="3">
			<tr>
				<td width="150"><img src="/img/fxcop/fxcop-warning.gif" /> Warnings</td>
				<td bgcolor="#FFFF00">
					<xsl:attribute name="width"><xsl:value-of select="$Warnings" /></xsl:attribute>
				</td>
				<td>
					<xsl:attribute name="width"><xsl:value-of select="500 - $Warnings" /></xsl:attribute>
				</td>
				<td><xsl:value-of select="$Warnings" /></td>
			</tr>
		</table>
		<br />
	</xsl:template>
</xsl:stylesheet>
