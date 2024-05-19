-- Lisätään yrityksen tiedot.
INSERT INTO yritys (yrityksen_nimi, katuosoite, postinumero, postitoimipaikka, email, puhelin)
	VALUES 
	('Vuokratoimistot Oy', 'Karjalankatu 3', '80200', 'Joensuu', 'vuokratoimistot@vuokratoimistot.fi', '050 1234567');

-- Lisätään yritykselle toimipisteitä.
INSERT INTO toimipiste (paikkakunta, toimipiste_nimi, katuosoite, postinumero, postitoimipaikka, puhelin, yritysID)
	VALUES 
	('Joensuu', 'Joensuu Karjalankatu', 'Karjalankatu 3', '80200', 'Joensuu', '050 1234567', '1'),
	('Joensuu', 'Joensuu Voimatie', 'Voimatie 2', '80100', 'Joensuu', '050 1111111', '1'),
	('Kuopio', 'Kuopio Savilahdentie', 'Savilahdentie 10', '70700', 'Kuopio', '050 2222222', '1'),
	('Helsinki', 'Helsinki Mannerheimintie', 'Mannerheimintie 9', '00100', 'Helsinki', '050 3333333', '1');

-- Lisätään toimipisteisiin vuokrattavia huoneita.
INSERT INTO huoneet (nimi, hinta, alv_prosentti, hlo_maara, toimipisteID)
	VALUES 
	('Joensuu Karjalankatu H1', '110', '24', '25', '1'),
	('Joensuu Karjalankatu H2', '100', '24', '20', '1'),
	('Joensuu Voimatie H1', '100', '24', '20', '2'),
	('Kuopio Savilahdentie H1', '90', '24', '18', '3'),
	('Helsinki Mannerheimintie H1', '125', '24', '30', '4');

-- Lisätään toimipisteisiin palveluita.
INSERT INTO palvelu (tuote, palvelun_hinta, alv_prosentti, maara, toimipisteID)
	VALUES 
	('kahvinkeitin', '10', '24', '2', '1'), 
	('siivous', '50', '24', '1', '1'), 
	('tablet-tietokone', '5', '24', '4', '1'), 
	('kahvinkeitin', '10', '24', '2', '2'), 
	('siivous', '50', '24', '1', '2'), 
	('kahvinkeitin', '10', '24', '3', '3'),
	('siivous', '50', '24', '1', '3'), 
	('siivous', '50', '24', '1', '4'), 
	('tablet-tietokone', '5', '24', '5', '4');

-- Lisätään työntekijöitä; kaytto_oikeus-kentässä 1 = admin-oikeudet, 2 = tavalliset käyttöoikeudet.
INSERT INTO tyontekija (nimi, osoite, puhelin, kayttajaID, salasana, kaytto_oikeus, yritysID)
	VALUES 
	('admin', 'N/A', 'N/A', 'admin', 'admin', '1', '1'), 
	('Maiju Mikkonen', 'Siltakatu 4 80100 Joensuu', '050 3333333', 'mmikkonen', 'abc123', '2', '1'), 
	('Eelis Hakkarainen', 'Pekkalankatu 21 B 3 Joensuu', '050 2727272', 'ehakkarainen', '1234ab', '2', '1'), 
	('Antti Lahti', 'Asemakatu 1 70100 Kuopio', '050 4444444', 'alahti', 'abcdefg', '2', '1'),
	('Krista Honkala', 'Ratapihantie 6 00520 Helsinki', '050 5656565', 'khonkala', '1a1a1a', '2', '1');
			
-- Sijoitetaan työntekijät toimipisteisiin.
INSERT INTO toimipisteen_tyontekija (toimipisteID, tyontekijaID)
	VALUES 
	('1', '2'), 
	('2', '3'),
	('3', '4'),
	('4', '5');

-- Lisätään asiakastietoja.
INSERT INTO asiakas (nimi, puhelin, katuosoite, postinumero, postitoimipaikka, sahkoposti)
	VALUES 
	('Heikki Korhonen', '050 7654321', 'Mutalantie 28', '80100', 'Joensuu', 'h.korhonen@mail.com'),
	('Lena Gröhn', '050 9898989', 'Piispansilta 11', '02230', 'Espoo', 'lgrhn@mail.com');

-- Lisätään varauksia.
INSERT INTO asiakkaan_varaus (varaus_alkaa, varaus_paattyy, huoneen_numeroID, varauspvm, lisatiedot,  asiakasID, tyontekijaID)
	VALUES 
	('2024-05-02', '2024-05-03', '3', '2023-04-22', 'Ei lisätietoja.', '1', '3'),
	('2024-05-10', '2024-05-15', '1', '2023-04-22', 'Päivittäissiivous klo 20 jälkeen.', '2', '2');

-- Lisätään tehtyihin varauksiin palveluita.
INSERT INTO varauksen_palvelut (palveluID, kpl, varausID)
	VALUES 
	('4', '1', '1'),
	('5', '1', '1'),
	('2', '1', '2');

-- Lisätään laskuja.
INSERT INTO lasku (laskutustapa, alv_euroina, veroton_summa, loppusumma, asiakasID, varausID)
	VALUES 
	('sähköpostilasku', '81.60', '340', '421.60', '1', '1'),
	('sähköpostilasku', '230.40', '960', '1190.40', '2', '2');