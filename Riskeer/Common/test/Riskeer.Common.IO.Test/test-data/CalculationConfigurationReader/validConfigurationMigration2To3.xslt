<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="berekening/@a">
        <!-- Change attribute value -->
        <xsl:attribute name="a">
            <xsl:value-of select="'test3'"/>
        </xsl:attribute>
    </xsl:template>
</xsl:stylesheet>