# HoehenlinienMitDelanayTriangulation




## Delaunay Triangulation
Um zu umgehen, dass die Messdaten immer als Rastermuster gegeben seien müssen, wird die Delaunay Triagulation angewendet um Dreiecke aus den Messdaten zu erstellen. Die Delaunay Triagulation sorgt zudem dafür, dass der kleinste Innenwinkel der Dreicke maximal ist. Dies dient zur besseren Interpolationsmöglichkeit. Das Ergebnis ist somit ein Dreiecksmuster, welchen aus zufälligen Messwerten erstellt wird. 

Die Datenstruktur wird für die Delaunay Triangulation angepasst. Es wird eine Klasse "Vector" erstellt. Ein Objekt dieser Klasse stellt ein Messwert dar. Somit wird aus der Liste aus double arrays, eine Liste aus vectors. Zudem werden für bestimmte Berechnungen (, auf welche später beim UML-Sequenzdiagramm näher eingegangen wird) die Klassen "Triagle"  und "StraightLine" erstellt. Ein "Triangle"-Objekt besteht aus 3 Vektoren, welche die Eckpunkte des Dreiecks darstellen. Ein "StraightLine"-Objekt besteht hingegen aus lediglich 2 Vektoren, wobei der erste Vektor für den Ortsvektor und der zweite Vektor für den Richtungsvektor der Geraden steht. Wenn die neuen Klassen dem Klassendiagramm des Projektes hinzugefügt werden, entsteht folgendes Diagramm.

![Klassendiagramm Delaunay Triangulation](https://github.com/JoBo33/Hoehenlinien/blob/main/Example-Pictures/Class-Diagram%20Delaunay.png "Beispiel Klassendiagramm")

  
Wenn man das UML-Sequenzdiagramm für Höhenlinien an die Delaunay Triangulation anpasst, erhält man folgendes Diagramm.

![Sequenzdiagramm Delaunay Triangulation](https://github.com/JoBo33/Hoehenlinien/blob/main/Example-Pictures/UML-Diagram%20Contour%20Lines%20with%20Delaunay%20triangulat.png "Beispiel Delaunay Triangulation")

Zu Beginn der Delaunay Triangulation wird ein "superTriangle" erstellt, welches alle Messwerte enthält. Die Delaunay Triagnulation findet grob gesagt in 2 verschachtelten For-Schleifen statt. Bei der ersten ist durch die gegebenden Messwerte zu iterieren. In jeder Iteration müssen die derzeitigen Dreiecke bestimmt werden. In der zweiten Schleife wird durch die zuvor bestimmten Dreiecke durch iteriert. Während desses werden die Umkreise der Dreiecke berechnet. Wenn der ausgewählte Punkt innerhalb des Kreises ist, wird das Dreieck zu den "suspectTriangles" hinzugefügt. Im Anschluss an die innere Schleife, werden Dreiecke herausgefiltert, die andere Dreiecke in sich drin haben. Außerdem wird überprüft, ob eine Kante Teil von 2 "suspectTriangles" ist. Ist dies der Fall, wird die Kante entfernt. Ebenso werden neue Kanten zwischen dem ausgewählten Punkt und den Eckpunkten der "suspectTriangles" hinzugefügt. Zuletzt werden alle Kanten entfernt, die eine Verbindung zum "superTriangle" darstellen, gelöscht.
