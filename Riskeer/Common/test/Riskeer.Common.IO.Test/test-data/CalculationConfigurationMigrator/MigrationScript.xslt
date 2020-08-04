<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" version="1.0" indent="yes" omit-xml-declaration="yes"/>
    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="root">
        <xsl:copy>
            <!-- copy everything inside 'root' node -->
            <xsl:apply-templates select="@* | node()"/>
            <!-- Add attribute -->
            <xsl:attribute name="test">true</xsl:attribute>
            <!-- Add element -->
            <xsl:element name="newNode"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>