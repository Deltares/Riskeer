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
          <gml:posList>0.35 10.642 10.1564 20.23</gml:posList>
        </gml:LineString>
      </asm:geometrie2D>
      <asm:lengte uom="m">100</asm:lengte>
      <asm:typeWaterkeringstelsel>Dijktraject</asm:typeWaterkeringstelsel>
    </asm:Waterkeringstelsel>
    <asm:Beoordelingsproces BeoordelingsprocesID="beoordelingsproces1" WaterkeringstelselIDRef="section1">
      <asm:beginJaarBeoordelingsronde>2017</asm:beginJaarBeoordelingsronde>
      <asm:eindJaarBeoordelingsronde>2023</asm:eindJaarBeoordelingsronde>
    </asm:Beoordelingsproces>
    <asm:Veiligheidsoordeel VeiligheidsoordeelID="veiligheidsoordeel_1" BeoordelingsprocesIDRef="beoordelingsproces1">
      <asm:toetsoordeelMetKansschatting>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:categorieTraject>NVT</asm:categorieTraject>
        <asm:faalkans>0.000124</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeelMetKansschatting>
      <asm:toetsoordeelZonderKansschatting>
        <asm:assemblagemethode>WBI-2B-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeelZonderKansschatting>
      <asm:veiligheidsoordeel>
        <asm:assemblagemethode>WBI-2C-1</asm:assemblagemethode>
        <asm:categorie>B</asm:categorie>
        <asm:status>VOLLDG</asm:status>
      </asm:veiligheidsoordeel>
    </asm:Veiligheidsoordeel>
    <asm:Toetsspoor ToetsspoorID="toetsspoorGABI" VeiligheidsoordeelIDRef="veiligheidsoordeel_1">
      <asm:typeToetsspoor>GABI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toets ToetsID="resultaat_GABI_1" ToetsspoorIDRef="toetsspoorGABI" WaterkeringsectieIDRef="vak_GABI_1">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:categorieVak>III-vak</asm:categorieVak>
        <asm:toets>GECBNTR</asm:toets>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:categorieVak>II-vak</asm:categorieVak>
        <asm:faalkans>0.5</asm:faalkans>
        <asm:toets>EENVDGETS</asm:toets>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:categorieVak>III-vak</asm:categorieVak>
        <asm:toets>TOETSOPMT</asm:toets>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:GecombineerdToetsoordeel GecombineerdToetsoordeelID="resultaat_gecombineerd_1" VeiligheidsoordeelIDRef="veiligheidsoordeel_1" WaterkeringsectieIDRef="vak_gecombineerd_1">
      <asm:toetsoordeelGecombineerd>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:toets>GECBNTRDV</asm:toets>
      </asm:toetsoordeelGecombineerd>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:categorieVak>III-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
        <asm:typeToetsspoor>HTKW</asm:typeToetsspoor>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
        <asm:typeToetsspoor>STPH</asm:typeToetsspoor>
      </asm:eindtoetsoordeelToetsspoor>
    </asm:GecombineerdToetsoordeel>
    <asm:Vakindeling VakindelingID="vakindelingGABI" ToetsspoorIDRef="toetsspoorGABI" />
    <asm:Vakindeling VakindelingID="vakindeling_gecombineerd" VeiligheidsoordeelIDRef="veiligheidsoordeel_1" />
    <asm:Waterkeringsectie gml:id="vak_GABI_1" VakindelingIDRef="vakindelingGABI">
      <asm:afstandBegin uom="m">0.12</asm:afstandBegin>
      <asm:afstandEinde uom="m">10.23</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0.23 0.24 10.23 10.24</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">14.142135623730951</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="vak_gecombineerd_1" VakindelingIDRef="vakindeling_gecombineerd">
      <asm:afstandBegin uom="m">0.12</asm:afstandBegin>
      <asm:afstandEinde uom="m">10.23</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0.23 0.24 10.23 10.24</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">14.142135623730951</asm:lengte>
      <asm:typeWaterkeringsectie>GECBNETSSTE</asm:typeWaterkeringsectie>
      <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
    </asm:Waterkeringsectie>
  </asm:featureMember>
</asm:Assemblage>