# Systeemtesten MySecureBackend API

**Datum:** 8 maart 2026  
**Tester:** Abdul  
**Tool:** Postman

---

## Systeemtest 1: Gebruiker kan registreren

**Acceptatiecriterium:**  
Een nieuwe gebruiker moet zich kunnen registreren met email en wachtwoord.

**Stappen:**
1. Open Postman
2. Nieuwe request: POST `http://localhost:5000/account/register`
3. Headers: `Content-Type: application/json`
4. Body (raw JSON):

5. Klik Send

**Verwachte Output:**
- Status Code: 200 OK
- Gebruiker is aangemaakt in database

**Werkelijk Resultaat:**
- Status: 200 OK

**Screenshot:** [PLAK HIER]

**Resultaat:** ☑ PASS

---

## Systeemtest 2: Gebruiker kan inloggen

**Acceptatiecriterium:**  
Een geregistreerde gebruiker moet kunnen inloggen en een access token ontvangen.

**Stappen:**
1. Postman request: POST `http://localhost:5000/account/login`
2. Headers: `Content-Type: application/json`
3. Body (raw JSON):

4. Klik Send

**Verwachte Output:**
- Status Code: 200 OK
- Response bevat `accessToken`
- Cookies worden gezet

**Werkelijk Resultaat:**
- Status: 200 OK
- Token ontvangen: N/A (authenticatie via cookies)
- Cookies gezet: JA ✅

**Screenshot:** [PLAK HIER]

**Resultaat:** ☑ PASS

---

## Systeemtest 3: Ingelogde gebruiker kan Environment2D aanmaken

**Acceptatiecriterium:**  
Een ingelogde gebruiker moet een nieuwe 2D-wereld kunnen aanmaken.

**Stappen:**
1. Zorg dat je ingelogd bent (voer Systeemtest 2 uit)
2. Postman request: POST `http://localhost:5000/environment2d`
3. Headers: `Content-Type: application/json`
4. Cookies van login worden automatisch meegestuurd
5. Body (raw JSON):

6. Klik Send

**Verwachte Output:**
- Status Code: 201 Created
- Response bevat nieuwe wereld met ID
- `userId` is automatisch ingevuld

**Werkelijk Resultaat:**
- Status: 201 Created
- Wereld aangemaakt: JA ✅

**Screenshot:** [PLAK SCREENSHOT HIER]

**Resultaat:** ☑ PASS

---

## Samenvatting

| Systeemtest | Acceptatiecriterium | Status |
|-------------|---------------------|--------|
| 1. Register | Gebruiker kan account aanmaken | ☑ PASS |
| 2. Login | Gebruiker kan inloggen | ☑ PASS |
| 3. Create Environment2D | Gebruiker kan wereld aanmaken | ☑ PASS |

**Conclusie:**  
VUL IN: ALLE TESTS PASSED 