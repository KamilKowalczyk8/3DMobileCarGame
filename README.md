# 🚗 Mobile Car Game - Endless Runner

Prosta gra mobilna typu "Endless Runner" stworzona w silniku Unity. Gracz steruje samochodem, omija przeszkody i zdobywa punkty w nieskończonym, proceduralnie generowanym świecie.

Projekt tworzony w celach edukacyjnych, z naciskiem na **Clean Code** i dobre praktyki programistyczne.

---

## 📸 Funkcjonalności

* **Nieskończona droga:** System proceduralnego generowania i niszczenia segmentów drogi (Object Pooling pattern).
* **Sterowanie Hybrydowe:**
    * 🖥️ **PC:** Klawiatura (Strzałki / WASD) - do testowania w edytorze.
    * 📱 **Mobile:** Dedykowane przyciski dotykowe (Gaz, Hamulec, Lewo, Prawo) oparte o system `EventTrigger`.
* **System Punktacji:** Punkty naliczane dynamicznie w zależności od prędkości gracza.
* **Fizyka:** Realistyczne zachowanie samochodu, wykrywanie kolizji z bandami i innymi autami.
* **UI (Interfejs):** Skalowalne menu, licznik prędkości, ekran Game Over z wynikiem.

---

## 🛠️ Technologie

* **Silnik:** Unity 6 (wersja 6000.0.x)
* **Język:** C#
* **Platforma docelowa:** Android (wymuszony tryb Landscape)
* **Wersjonowanie:** Git & GitHub

---

## 🎮 Sterowanie (Mobile)

Gra wykorzystuje przyciski ekranowe (UI):

| Przycisk | Akcja | Opis |
| :--- | :--- | :--- |
| **Zielony (Gaz)** | Przyspieszanie | Trzymaj, aby jechać szybciej. Puszczenie powoduje powolne wytracanie prędkości. |
| **Czerwony (Hamulec)** | Hamowanie | Wciśnij, aby zwolnić lub cofać. |
| **Niebieski (Lewo)** | Skręt w lewo | Zmiana pasa ruchu w lewo. |
| **Fioletowy (Prawo)** | Skręt w prawo | Zmiana pasa ruchu w prawo. |

---

## 📂 Struktura Projektu (Główne Skrypty)

Staramy się trzymać porządek w kodzie:

* `CarController.cs` - Fizyka pojazdu, obsługa wejścia (Input) oraz logika poruszania się.
* `RoadManager.cs` - Zarządzanie segmentami drogi (spawnowanie nowych, usuwanie starych).
* `GameOverManager.cs` - Logika stanu gry (przegrana, restart sceny).
* `MainMenu.cs` - Obsługa menu startowego i nawigacja między scenami.

---

## 🚀 Jak uruchomić projekt?

1.  Sklonuj repozytorium:
    ```bash
    git clone [https://github.com/KamilKowalczyk8/MobileCarGame.git](https://github.com/KamilKowalczyk8/MobileCarGame.git)
    ```
2.  Otwórz **Unity Hub**.
3.  Kliknij **Add** i wybierz folder z pobranym projektem.
4.  Otwórz scenę: `Assets/Scenes/Menu.unity`.

---

## 📝 Autor

Projekt stworzony w ramach nauki Unity i C#.
