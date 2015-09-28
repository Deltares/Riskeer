<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions">
	<xsl:template match="/topics">
		<!-- Begin our initial task list. -->
		<!--ol-->
			<!-- Now, apply the task template to all top-level tasks in the task list. -->
			<xsl:apply-templates select="topic"/>
		<!--/ol-->
	</xsl:template>
	<!-- Matches a task node.  -->
	<xsl:template match="topic">
		<!-- Output the task. -->
		<xsl:if test="starts-with(@id,'T:')">
			<li>
				<a>
					<xsl:attribute name="href"><xsl:value-of select="concat('html/',@file,'.htm')"/></xsl:attribute>
					<!--xsl:attribute name="href">
    <xsl:value-of select="concat('http://wl06978/',@file)"/>
  </xsl:attribute-->
					<xsl:value-of select="substring-after(@id,concat('T:',@project,'.'))"/>
				</a>
				</li>
				<!-- Check to see if there are any sub tasks that need their own list. -->
				<xsl:if test="topic">
					<!-- Start a new sub-task list and then RECURSIVELY apply the task template to each of the sub-task nodes. -->
					<!--ol-->
						<xsl:apply-templates select="topic"/>
					<!--/ol-->
				</xsl:if>
			<!--/li-->
		</xsl:if>
		<xsl:if test="not(starts-with(@id,'T:')) and (topic)">
								<xsl:apply-templates select="topic"/>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
