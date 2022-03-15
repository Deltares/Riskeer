<?xml version="1.0" encoding="utf-8"?>
<asm:Assemblage xmlns:gml="http://www.opengis.net/gml/3.2" gml:id="assemblage_1" xmlns:asm="http://localhost/standaarden/assemblage">
  <gml:boundedBy>
    <gml:Envelope>
      <gml:lowerCorner>12 34</gml:lowerCorner>
      <gml:upperCorner>56.053 78.0002345</gml:upperCorner>
    </gml:Envelope>
  </gml:boundedBy>
  <asm:featureMember>
    <asm:Waterkeringstelsel gml:id="section1">
      <asm:naam>Traject A</asm:naam>
      <asm:geometrie2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0 0 100 0</gml:posList>
        </gml:LineString>
      </asm:geometrie2D>
      <asm:lengte uom="m">100</asm:lengte>
      <asm:typeWaterkeringstelsel>DKTRJCT</asm:typeWaterkeringstelsel>
    </asm:Waterkeringstelsel>
    <asm:Beoordelingsproces BeoordelingsprocesID="beoordelingsproces1" WaterkeringstelselIDRef="section1">
      <asm:beginJaarBeoordelingsronde>2023</asm:beginJaarBeoordelingsronde>
      <asm:eindJaarBeoordelingsronde>2035</asm:eindJaarBeoordelingsronde>
    </asm:Beoordelingsproces>
    <asm:Veiligheidsoordeel VeiligheidsoordeelID="veiligheidsoordeel_1" BeoordelingsprocesIDRef="beoordelingsproces1">
      <asm:veiligheidsoordeel>
        <asm:assemblagemethode>WBI-2B-1</asm:assemblagemethode>
        <asm:categorie>B</asm:categorie>
        <asm:faalkans>0.00068354</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:veiligheidsoordeel>
    </asm:Veiligheidsoordeel>
    <asm:Faalmechanisme FaalmechanismeID="toetsspoorGABI" VeiligheidsoordeelIDRef="veiligheidsoordeel_1">
      <asm:typeFaalmechanisme>GENRK</asm:typeFaalmechanisme>
      <asm:generiekFaalmechanisme>GABI</asm:generiekFaalmechanisme>
      <asm:analyseFaalmechanisme>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:faalkans>0.08419</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseFaalmechanisme>
    </asm:Faalmechanisme>
    <asm:Faalanalyse FaalanalyseID="resultaat_GABI_1" FaalmechanismeIDRef="toetsspoorGABI" WaterkeringsectieIDRef="vak_GABI_1">
      <asm:analyseVak>
        <asm:assemblagemethode>WBI-0A-2</asm:assemblagemethode>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:faalkans>0.00073</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseVak>
    </asm:Faalanalyse>
    <asm:FaalanalyseGecombineerd FaalanalyseGecombineerdID="resultaat_gecombineerd_1" VeiligheidsoordeelIDRef="veiligheidsoordeel_1" WaterkeringsectieIDRef="vak_gecombineerd_1">
      <asm:analyseGecombineerdDeelvak>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:duidingsklasse>+I</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseGecombineerdDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>HTKW</asm:typeFaalmechanisme>
        <asm:duidingsklasse>+III</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
      <asm:analyseDeelvak>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeFaalmechanisme>STPH</asm:typeFaalmechanisme>
        <asm:duidingsklasse>+II</asm:duidingsklasse>
        <asm:status>VOLLDG</asm:status>
      </asm:analyseDeelvak>
    </asm:FaalanalyseGecombineerd>
    <asm:Vakindeling VakindelingID="vakindelingGABI" />
    <asm:Vakindeling VakindelingID="vakindeling_gecombineerd" />
    <asm:Deelvak gml:id="vak_GABI_1" VakindelingIDRef="vakindelingGABI">
      <asm:afstandBegin uom="m">0.12</asm:afstandBegin>
      <asm:afstandEinde uom="m">10.23</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0.23 0.24 10.23 10.24</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">14.142135623730951</asm:lengte>
      <asm:typeWaterkeringsectie>FAALMVK</asm:typeWaterkeringsectie>
    </asm:Deelvak>
    <asm:Deelvak gml:id="vak_gecombineerd_1" VakindelingIDRef="vakindeling_gecombineerd">
      <asm:afstandBegin uom="m">0.12</asm:afstandBegin>
      <asm:afstandEinde uom="m">10.23</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0.23 0.24 10.23 10.24</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">14.142135623730951</asm:lengte>
      <asm:typeWaterkeringsectie>DEELVK</asm:typeWaterkeringsectie>
      <asm:assemblagemethode>WBI-3A-1</asm:assemblagemethode>
    </asm:Deelvak>
  </asm:featureMember>
</asm:Assemblage>