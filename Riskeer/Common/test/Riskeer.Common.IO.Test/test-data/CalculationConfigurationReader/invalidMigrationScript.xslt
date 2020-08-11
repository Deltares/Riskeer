<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" version="1.0" indent="yes" omit-xml-declaration="yes"/>
    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="berekening">
        <xsl:copy>
            <!-- copy everything inside 'berekening' node -->
            <xsl:apply-templates select="@* | node()"/>
            <!-- Add attribute -->
            <xsl:attribute name="test">true</xsl:attribute>
        </xsl:copy>

        <!-- Add element (invalid) -->
        <xsl:element />
    </xsl:template>
</xsl:stylesheet>