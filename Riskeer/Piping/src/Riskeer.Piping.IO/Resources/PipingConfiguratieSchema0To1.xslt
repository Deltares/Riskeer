<?xml version="1.0" encoding="UTF-8"?><!--
Copyright (C) Stichting Deltares 2019. All rights reserved.

This file is part of Riskeer.

Riskeer is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.

All names, logos, and references to "Deltares" are registered trademarks of
Stichting Deltares and remain full property of Stichting Deltares at all times.
All rights reserved.
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="node()|@*">
      <xsl:copy>
          <xsl:apply-templates select="node()|@*"/>
      </xsl:copy>
  </xsl:template>

  <!--Add version attribute.-->
  <xsl:template match="configuratie">
    <xsl:copy>
      <xsl:attribute name="versie">1</xsl:attribute>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>

  <!--Add 'semi-probabilistisch' element, and move 'hrlocatie' and 'toetspeil' to it.-->
  <xsl:template match="berekening">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()[not(self::toetspeil or self::hrlocatie)]"/>
      <xsl:element name="semi-probabilistisch">
        <xsl:apply-templates select="hrlocatie"/>
        <xsl:apply-templates select="toetspeil"/>
      </xsl:element>
    </xsl:copy>
  </xsl:template>

  <!--Rename 'hrlocatie' to 'hblocatie'.-->
  <xsl:template match="hrlocatie">
    <xsl:element name="hblocatie">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <!--Rename 'toetspeil' to 'waterstand'.-->
  <xsl:template match="toetspeil">
    <xsl:element name="waterstand">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <!--Rename 'polderpeil' stochast to 'binnendijksewaterstand'.-->
  <xsl:template match="stochast/@naam[.='polderpeil']">
    <xsl:attribute name="naam">
      <xsl:value-of select="'binnendijksewaterstand'"/>
    </xsl:attribute>
  </xsl:template>

  <!--Adjust value of 'bijdrage' in 'scenario'.-->
  <xsl:template match="scenario/bijdrage">
    <xsl:copy>
      <xsl:choose>
        <xsl:when test=".&gt;100">
          <xsl:number value="100"/>
        </xsl:when>
        <xsl:when test=".&lt;0">
          <xsl:number value="0"/>
        </xsl:when>
        <xsl:when test=".='NaN'">
          <xsl:number value="0"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>