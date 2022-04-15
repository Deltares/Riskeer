<?xml version="1.0" encoding="utf-8"?>
<asm:Assemblage xmlns:gml="http://www.opengis.net/gml/3.2" gml:id="Assemblage.0" xmlns:asm="http://localhost/standaarden/assemblage">
  <gml:boundedBy>
    <gml:Envelope>
      <gml:lowerCorner>0 0</gml:lowerCorner>
      <gml:upperCorner>3 4</gml:upperCorner>
    </gml:Envelope>
  </gml:boundedBy>
  <asm:featureMember>
    <asm:Waterkeringstelsel gml:id="Wks.assessmentSectionId">
      <asm:naam>assessmentSectionName</asm:naam>
      <asm:geometrie2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0 0 3 4</gml:posList>
        </gml:LineString>
      </asm:geometrie2D>
      <asm:lengte uom="m">5</asm:lengte>
      <asm:typeWaterkeringstelsel>DKTRJCT</asm:typeWaterkeringstelsel>
    </asm:Waterkeringstelsel>
    <asm:Beoordelingsproces BeoordelingsprocesID="Bp.0" WaterkeringstelselIDRef="Wks.assessmentSectionId">
      <asm:beginJaarBeoordelingsronde>2023</asm:beginJaarBeoordelingsronde>
      <asm:eindJaarBeoordelingsronde>2035</asm:eindJaarBeoordelingsronde>
    </asm:Beoordelingsproces>
    <asm:Veiligheidsoordeel VeiligheidsoordeelID="Vo.0" BeoordelingsprocesIDRef="Bp.0">
      <asm:assemblagemethode>WBI-2B-1</asm:assemblagemethode>
      <asm:categorie>A+</asm:categorie>
      <asm:faalkans>0.14</asm:faalkans>
      <asm:status>VOLLDG</asm:status>
    </asm:Veiligheidsoordeel>
    <asm:Faalmechanisme FaalmechanismeID="Fm.0" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>STPH</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.1" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>GEKB</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.2" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>STBI</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.3" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>STMI</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.4" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>ZST</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.5" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>AGK</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.6" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>AWO</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.7" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>GEBU</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.8" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>GABU</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.9" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>GABI</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.10" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>HTKW</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.11" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>BSKW</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.12" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>PKW</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.13" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>STKWp</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.14" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>SPECFK</asm:typeFaalmechanisme>
      <asm:specifiekFaalmechanisme>Specific failure mechanism 1</asm:specifiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalmechanisme FaalmechanismeID="Fm.15" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeFaalmechanisme>SPECFK</asm:typeFaalmechanisme>
      <asm:specifiekFaalmechanisme>Specific failure mechanism 2</asm:specifiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalanalyse FaalanalyseID="Fa.0" FaalmechanismeIDRef="Fm.0" WaterkeringsectieIDRef="Bv.0">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.1" FaalmechanismeIDRef="Fm.0" WaterkeringsectieIDRef="Bv.1">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.2" FaalmechanismeIDRef="Fm.1" WaterkeringsectieIDRef="Bv.2">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.3" FaalmechanismeIDRef="Fm.1" WaterkeringsectieIDRef="Bv.3">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.4" FaalmechanismeIDRef="Fm.2" WaterkeringsectieIDRef="Bv.4">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.5" FaalmechanismeIDRef="Fm.2" WaterkeringsectieIDRef="Bv.5">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.6" FaalmechanismeIDRef="Fm.3" WaterkeringsectieIDRef="Bv.6">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.7" FaalmechanismeIDRef="Fm.3" WaterkeringsectieIDRef="Bv.7">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.8" FaalmechanismeIDRef="Fm.4" WaterkeringsectieIDRef="Bv.8">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.9" FaalmechanismeIDRef="Fm.4" WaterkeringsectieIDRef="Bv.9">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.10" FaalmechanismeIDRef="Fm.5" WaterkeringsectieIDRef="Bv.10">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.11" FaalmechanismeIDRef="Fm.5" WaterkeringsectieIDRef="Bv.11">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.12" FaalmechanismeIDRef="Fm.6" WaterkeringsectieIDRef="Bv.12">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.13" FaalmechanismeIDRef="Fm.6" WaterkeringsectieIDRef="Bv.13">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.14" FaalmechanismeIDRef="Fm.7" WaterkeringsectieIDRef="Bv.14">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.15" FaalmechanismeIDRef="Fm.7" WaterkeringsectieIDRef="Bv.15">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.16" FaalmechanismeIDRef="Fm.8" WaterkeringsectieIDRef="Bv.16">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.17" FaalmechanismeIDRef="Fm.8" WaterkeringsectieIDRef="Bv.17">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.18" FaalmechanismeIDRef="Fm.9" WaterkeringsectieIDRef="Bv.18">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.19" FaalmechanismeIDRef="Fm.9" WaterkeringsectieIDRef="Bv.19">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.20" FaalmechanismeIDRef="Fm.10" WaterkeringsectieIDRef="Bv.20">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.21" FaalmechanismeIDRef="Fm.10" WaterkeringsectieIDRef="Bv.21">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.22" FaalmechanismeIDRef="Fm.11" WaterkeringsectieIDRef="Bv.22">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.23" FaalmechanismeIDRef="Fm.11" WaterkeringsectieIDRef="Bv.23">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.24" FaalmechanismeIDRef="Fm.12" WaterkeringsectieIDRef="Bv.24">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.25" FaalmechanismeIDRef="Fm.12" WaterkeringsectieIDRef="Bv.25">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.26" FaalmechanismeIDRef="Fm.13" WaterkeringsectieIDRef="Bv.26">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.27" FaalmechanismeIDRef="Fm.13" WaterkeringsectieIDRef="Bv.27">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.28" FaalmechanismeIDRef="Fm.14" WaterkeringsectieIDRef="Bv.28">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.29" FaalmechanismeIDRef="Fm.14" WaterkeringsectieIDRef="Bv.29">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.30" FaalmechanismeIDRef="Fm.15" WaterkeringsectieIDRef="Bv.30">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:Faalanalyse FaalanalyseID="Fa.31" FaalmechanismeIDRef="Fm.15" WaterkeringsectieIDRef="Bv.31">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:faalkans>0.1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:FaalanalyseGecombineerd FaalanalyseGecombineerdID="Gf.0" VeiligheidsoordeelIDRef="Vo.0" WaterkeringsectieIDRef="Bv.32">
      <asm:analyseGecombineerdDeelvak>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseGecombineerdDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STPH</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GEKB</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STBI</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STMI</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>ZST</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>AGK</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>AWO</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GEBU</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GABU</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GABI</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>HTKW</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>BSKW</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>PKW</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STKWp</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>SPECFK</asm:typeFaalmechanisme>
        <asm:specifiekFaalmechanisme>Specific failure mechanism 1</asm:specifiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>SPECFK</asm:typeFaalmechanisme>
        <asm:specifiekFaalmechanisme>Specific failure mechanism 2</asm:specifiekFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
    </asm:FaalanalyseGecombineerd>
    <asm:FaalanalyseGecombineerd FaalanalyseGecombineerdID="Gf.1" VeiligheidsoordeelIDRef="Vo.0" WaterkeringsectieIDRef="Bv.33">
      <asm:analyseGecombineerdDeelvak>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseGecombineerdDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STPH</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GEKB</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STBI</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STMI</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>ZST</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>AGK</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>AWO</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GEBU</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GABU</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>GABI</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>HTKW</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>BSKW</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>PKW</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
        <asm:generiekFaalmechanisme>STKWp</asm:generiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>SPECFK</asm:typeFaalmechanisme>
        <asm:specifiekFaalmechanisme>Specific failure mechanism 1</asm:specifiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>SPECFK</asm:typeFaalmechanisme>
        <asm:specifiekFaalmechanisme>Specific failure mechanism 2</asm:specifiekFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
    </asm:FaalanalyseGecombineerd>
    <asm:Vakindeling VakindelingID="Vi.0" />
    <asm:Vakindeling VakindelingID="Vi.1" />
    <asm:Vakindeling VakindelingID="Vi.2" />
    <asm:Vakindeling VakindelingID="Vi.3" />
    <asm:Vakindeling VakindelingID="Vi.4" />
    <asm:Vakindeling VakindelingID="Vi.5" />
    <asm:Vakindeling VakindelingID="Vi.6" />
    <asm:Vakindeling VakindelingID="Vi.7" />
    <asm:Vakindeling VakindelingID="Vi.8" />
    <asm:Vakindeling VakindelingID="Vi.9" />
    <asm:Vakindeling VakindelingID="Vi.10" />
    <asm:Vakindeling VakindelingID="Vi.11" />
    <asm:Vakindeling VakindelingID="Vi.12" />
    <asm:Vakindeling VakindelingID="Vi.13" />
    <asm:Vakindeling VakindelingID="Vi.14" />
    <asm:Vakindeling VakindelingID="Vi.15" />
    <asm:Vakindeling VakindelingID="Vi.16" />
    <asm:Deelvak gml:id="Bv.0" VakindelingIDRef="Vi.0">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.1" VakindelingIDRef="Vi.0">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.2" VakindelingIDRef="Vi.1">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.3" VakindelingIDRef="Vi.1">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.4" VakindelingIDRef="Vi.2">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.5" VakindelingIDRef="Vi.2">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.6" VakindelingIDRef="Vi.3">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.7" VakindelingIDRef="Vi.3">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.8" VakindelingIDRef="Vi.4">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.9" VakindelingIDRef="Vi.4">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.10" VakindelingIDRef="Vi.5">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.11" VakindelingIDRef="Vi.5">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.12" VakindelingIDRef="Vi.6">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.13" VakindelingIDRef="Vi.6">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.14" VakindelingIDRef="Vi.7">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.15" VakindelingIDRef="Vi.7">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.16" VakindelingIDRef="Vi.8">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.17" VakindelingIDRef="Vi.8">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.18" VakindelingIDRef="Vi.9">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.19" VakindelingIDRef="Vi.9">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.20" VakindelingIDRef="Vi.10">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.21" VakindelingIDRef="Vi.10">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.22" VakindelingIDRef="Vi.11">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.23" VakindelingIDRef="Vi.11">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.24" VakindelingIDRef="Vi.12">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.25" VakindelingIDRef="Vi.12">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.26" VakindelingIDRef="Vi.13">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.27" VakindelingIDRef="Vi.13">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.28" VakindelingIDRef="Vi.14">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.29" VakindelingIDRef="Vi.14">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.30" VakindelingIDRef="Vi.15">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.31" VakindelingIDRef="Vi.15">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.32" VakindelingIDRef="Vi.16">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">2.5</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0 0 1.5 2</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">2.5</asm:lengte>
      <asm:typeWaterkeringsectie>DEELVK</asm:typeWaterkeringsectie>
      <asm:assemblagemethode>WBI-3A-1</asm:assemblagemethode>
    </asm:Deelvak>
    <asm:Deelvak gml:id="Bv.33" VakindelingIDRef="Vi.16">
      <asm:afstandBegin uom="m">2.5</asm:afstandBegin>
      <asm:afstandEinde uom="m">5</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>1.5 2 3 4</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">2.5</asm:lengte>
      <asm:typeWaterkeringsectie>DEELVK</asm:typeWaterkeringsectie>
      <asm:assemblagemethode>WBI-3A-1</asm:assemblagemethode>
    </asm:Deelvak>
  </asm:featureMember>
</asm:Assemblage>