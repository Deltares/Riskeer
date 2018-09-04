<?xml version="1.0" encoding="utf-8"?>
<asm:Assemblage xmlns:gml="http://www.opengis.net/gml/3.2" gml:id="Assemblage.0" xmlns:asm="http://localhost/standaarden/assemblage">
  <gml:boundedBy>
    <gml:Envelope>
      <gml:lowerCorner>1 1</gml:lowerCorner>
      <gml:upperCorner>2 2</gml:upperCorner>
    </gml:Envelope>
  </gml:boundedBy>
  <asm:featureMember>
    <asm:Waterkeringstelsel gml:id="Wks.assessmentSectionId">
      <asm:naam>assessmentSectionName</asm:naam>
      <asm:geometrie2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>1 1 2 2</gml:posList>
        </gml:LineString>
      </asm:geometrie2D>
      <asm:lengte uom="m">1.4142135623730952</asm:lengte>
      <asm:typeWaterkeringstelsel>Dijktraject</asm:typeWaterkeringstelsel>
    </asm:Waterkeringstelsel>
    <asm:Beoordelingsproces BeoordelingsprocesID="Bp.1" WaterkeringstelselIDRef="Wks.assessmentSectionId">
      <asm:beginJaarBeoordelingsronde>2017</asm:beginJaarBeoordelingsronde>
      <asm:eindJaarBeoordelingsronde>2023</asm:eindJaarBeoordelingsronde>
    </asm:Beoordelingsproces>
    <asm:Veiligheidsoordeel VeiligheidsoordeelID="Vo.2" BeoordelingsprocesIDRef="Bp.1">
      <asm:veiligheidsoordeel>
        <asm:assemblagemethode>WBI-2C-1</asm:assemblagemethode>
        <asm:categorie>A</asm:categorie>
        <asm:status>VOLLDG</asm:status>
      </asm:veiligheidsoordeel>
      <asm:toetsoordeelMetKansschatting>
        <asm:assemblagemethode>WBI-2B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>0.75</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeelMetKansschatting>
      <asm:toetsoordeelZonderKansschatting>
        <asm:assemblagemethode>WBI-2A-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeelZonderKansschatting>
    </asm:Veiligheidsoordeel>
    <asm:Toetsspoor ToetsspoorID="Ts.3" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>STPH</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROBEX</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.9" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>STBI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROBEX</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.15" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>GEKB</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.21" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>HTKW</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.27" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>BSKW</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.33" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>STKWp</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.39" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>ZST</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.45" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>AGK</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.51" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>GEBU</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.57" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>DA</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>NVT</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.59" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>STBU</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.65" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>STMI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.71" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>GABU</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.77" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>GABI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.83" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>PKW</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.89" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>AWO</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.95" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>STKWl</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.101" VeiligheidsoordeelIDRef="Vo.2">
      <asm:typeToetsspoor>INN</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toets ToetsID="T.6" ToetsspoorIDRef="Ts.3" WaterkeringsectieIDRef="Wks.5">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.8" ToetsspoorIDRef="Ts.3" WaterkeringsectieIDRef="Wks.7">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.12" ToetsspoorIDRef="Ts.9" WaterkeringsectieIDRef="Wks.11">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.14" ToetsspoorIDRef="Ts.9" WaterkeringsectieIDRef="Wks.13">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.18" ToetsspoorIDRef="Ts.15" WaterkeringsectieIDRef="Wks.17">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.20" ToetsspoorIDRef="Ts.15" WaterkeringsectieIDRef="Wks.19">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.24" ToetsspoorIDRef="Ts.21" WaterkeringsectieIDRef="Wks.23">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.26" ToetsspoorIDRef="Ts.21" WaterkeringsectieIDRef="Wks.25">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.30" ToetsspoorIDRef="Ts.27" WaterkeringsectieIDRef="Wks.29">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.32" ToetsspoorIDRef="Ts.27" WaterkeringsectieIDRef="Wks.31">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.36" ToetsspoorIDRef="Ts.33" WaterkeringsectieIDRef="Wks.35">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.38" ToetsspoorIDRef="Ts.33" WaterkeringsectieIDRef="Wks.37">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.42" ToetsspoorIDRef="Ts.39" WaterkeringsectieIDRef="Wks.41">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.44" ToetsspoorIDRef="Ts.39" WaterkeringsectieIDRef="Wks.43">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.48" ToetsspoorIDRef="Ts.45" WaterkeringsectieIDRef="Wks.47">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.50" ToetsspoorIDRef="Ts.45" WaterkeringsectieIDRef="Wks.49">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.54" ToetsspoorIDRef="Ts.51" WaterkeringsectieIDRef="Wks.53">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.56" ToetsspoorIDRef="Ts.51" WaterkeringsectieIDRef="Wks.55">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.62" ToetsspoorIDRef="Ts.59" WaterkeringsectieIDRef="Wks.61">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-7</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.64" ToetsspoorIDRef="Ts.59" WaterkeringsectieIDRef="Wks.63">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-7</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.68" ToetsspoorIDRef="Ts.65" WaterkeringsectieIDRef="Wks.67">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.70" ToetsspoorIDRef="Ts.65" WaterkeringsectieIDRef="Wks.69">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.74" ToetsspoorIDRef="Ts.71" WaterkeringsectieIDRef="Wks.73">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.76" ToetsspoorIDRef="Ts.71" WaterkeringsectieIDRef="Wks.75">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.80" ToetsspoorIDRef="Ts.77" WaterkeringsectieIDRef="Wks.79">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.82" ToetsspoorIDRef="Ts.77" WaterkeringsectieIDRef="Wks.81">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.86" ToetsspoorIDRef="Ts.83" WaterkeringsectieIDRef="Wks.85">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.88" ToetsspoorIDRef="Ts.83" WaterkeringsectieIDRef="Wks.87">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.92" ToetsspoorIDRef="Ts.89" WaterkeringsectieIDRef="Wks.91">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.94" ToetsspoorIDRef="Ts.89" WaterkeringsectieIDRef="Wks.93">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.98" ToetsspoorIDRef="Ts.95" WaterkeringsectieIDRef="Wks.97">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.100" ToetsspoorIDRef="Ts.95" WaterkeringsectieIDRef="Wks.99">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.104" ToetsspoorIDRef="Ts.101" WaterkeringsectieIDRef="Wks.103">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.106" ToetsspoorIDRef="Ts.101" WaterkeringsectieIDRef="Wks.105">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:GecombineerdToetsoordeel GecombineerdToetsoordeelID="Gto.109" VeiligheidsoordeelIDRef="Vo.2" WaterkeringsectieIDRef="Wks.108">
      <asm:toetsoordeelGecombineerd>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>III-vak</asm:categorieVak>
      </asm:toetsoordeelGecombineerd>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STPH</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GEKB</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STBI</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STBU</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STMI</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>ZST</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>AGK</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>AWO</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GEBU</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GABU</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GABI</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>HTKW</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>BSKW</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>PKW</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STKWp</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STKWl</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>DA</asm:typeToetsspoor>
        <asm:categorieVak>NVT</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>INN</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
    </asm:GecombineerdToetsoordeel>
    <asm:Vakindeling VakindelingID="Vi.4" ToetsspoorIDRef="Ts.3" />
    <asm:Vakindeling VakindelingID="Vi.10" ToetsspoorIDRef="Ts.9" />
    <asm:Vakindeling VakindelingID="Vi.16" ToetsspoorIDRef="Ts.15" />
    <asm:Vakindeling VakindelingID="Vi.22" ToetsspoorIDRef="Ts.21" />
    <asm:Vakindeling VakindelingID="Vi.28" ToetsspoorIDRef="Ts.27" />
    <asm:Vakindeling VakindelingID="Vi.34" ToetsspoorIDRef="Ts.33" />
    <asm:Vakindeling VakindelingID="Vi.40" ToetsspoorIDRef="Ts.39" />
    <asm:Vakindeling VakindelingID="Vi.46" ToetsspoorIDRef="Ts.45" />
    <asm:Vakindeling VakindelingID="Vi.52" ToetsspoorIDRef="Ts.51" />
    <asm:Vakindeling VakindelingID="Vi.58" ToetsspoorIDRef="Ts.57" />
    <asm:Vakindeling VakindelingID="Vi.60" ToetsspoorIDRef="Ts.59" />
    <asm:Vakindeling VakindelingID="Vi.66" ToetsspoorIDRef="Ts.65" />
    <asm:Vakindeling VakindelingID="Vi.72" ToetsspoorIDRef="Ts.71" />
    <asm:Vakindeling VakindelingID="Vi.78" ToetsspoorIDRef="Ts.77" />
    <asm:Vakindeling VakindelingID="Vi.84" ToetsspoorIDRef="Ts.83" />
    <asm:Vakindeling VakindelingID="Vi.90" ToetsspoorIDRef="Ts.89" />
    <asm:Vakindeling VakindelingID="Vi.96" ToetsspoorIDRef="Ts.95" />
    <asm:Vakindeling VakindelingID="Vi.102" ToetsspoorIDRef="Ts.101" />
    <asm:Vakindeling VakindelingID="Vi.107" VeiligheidsoordeelIDRef="Vo.2" />
    <asm:Waterkeringsectie gml:id="Wks.5" VakindelingIDRef="Vi.4">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.7" VakindelingIDRef="Vi.4">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.11" VakindelingIDRef="Vi.10">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.13" VakindelingIDRef="Vi.10">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.17" VakindelingIDRef="Vi.16">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.19" VakindelingIDRef="Vi.16">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.23" VakindelingIDRef="Vi.22">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.25" VakindelingIDRef="Vi.22">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.29" VakindelingIDRef="Vi.28">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.31" VakindelingIDRef="Vi.28">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.35" VakindelingIDRef="Vi.34">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.37" VakindelingIDRef="Vi.34">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.41" VakindelingIDRef="Vi.40">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.43" VakindelingIDRef="Vi.40">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.47" VakindelingIDRef="Vi.46">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.49" VakindelingIDRef="Vi.46">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.53" VakindelingIDRef="Vi.52">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.55" VakindelingIDRef="Vi.52">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.61" VakindelingIDRef="Vi.60">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.63" VakindelingIDRef="Vi.60">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.67" VakindelingIDRef="Vi.66">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.69" VakindelingIDRef="Vi.66">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.73" VakindelingIDRef="Vi.72">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.75" VakindelingIDRef="Vi.72">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.79" VakindelingIDRef="Vi.78">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.81" VakindelingIDRef="Vi.78">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.85" VakindelingIDRef="Vi.84">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.87" VakindelingIDRef="Vi.84">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.91" VakindelingIDRef="Vi.90">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.93" VakindelingIDRef="Vi.90">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.97" VakindelingIDRef="Vi.96">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.99" VakindelingIDRef="Vi.96">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.103" VakindelingIDRef="Vi.102">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.105" VakindelingIDRef="Vi.102">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.108" VakindelingIDRef="Vi.107">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">1</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>1 1 1.70710678118655 1.70710678118655</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">0.99999999999999989</asm:lengte>
      <asm:typeWaterkeringsectie>GECBNETSSTE</asm:typeWaterkeringsectie>
      <asm:assemblagemethode>WBI-3A-1</asm:assemblagemethode>
    </asm:Waterkeringsectie>
  </asm:featureMember>
</asm:Assemblage>