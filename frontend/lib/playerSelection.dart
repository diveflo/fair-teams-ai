import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:frontend/apiService.dart';
import 'package:frontend/model/player.dart';

class PlayerSelection extends StatefulWidget {
  @override
  _PlayerSelectionState createState() => _PlayerSelectionState();
}

class _PlayerSelectionState extends State<PlayerSelection> {
  List<Player> players;
  List<Player> team1;
  List<Player> team2;
  bool isLoading;

  @override
  void initState() {
    players = List<Player>();
    team1 = List<Player>();
    team2 = List<Player>();
    players.add(Player(name: "Flo", steamID: "76561197973591119"));
    players.add(Player(name: "Hubi", steamID: "76561198258023370"));
    players.add(Player(name: "Alex", steamID: "76561198011775117"));
    players.add(Player(name: "Sandy", steamID: "76561198011654217"));
    players.add(Player(name: "Markus", steamID: "76561197984050254"));
    players.add(Player(name: "Andi", steamID: "76561199045573415"));
    players.add(Player(name: "Martin", steamID: "76561197978519504"));
    players.add(Player(name: "Ferdy", steamID: "76561198031200891"));
    // players.add(Player(name: "Niggo"));
    players.add(Player(name: "Stefan", steamID: "76561198058595736"));
    players.add(Player(name: "Uwe", steamID: "76561198053826525"));

    isLoading = false;

    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: [
        Padding(
          padding: const EdgeInsets.all(20.0),
          child: Text(
            "Choose the players!",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
        ),
        SizedBox(
          height: 30,
        ),
        Expanded(
          flex: 2,
          child: Container(
            width: 200,
            child: ListView.builder(
              itemCount: players.length,
              itemBuilder: (BuildContext context, int index) {
                return new CheckboxListTile(
                  title: Text(players[index].name,
                      style: Theme.of(context).primaryTextTheme.bodyText1),
                  value: players[index].isSelected,
                  onChanged: (bool value) {
                    setState(() {
                      players[index].isSelected = value;
                    });
                  },
                );
              },
            ),
          ),
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Padding(
              padding: EdgeInsets.all(10),
              child: RaisedButton(
                color: Colors.lime,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20)),
                child: Padding(
                  padding: const EdgeInsets.all(10.0),
                  child: Text(
                    "Scramble",
                    style: TextStyle(fontSize: 20),
                  ),
                ),
                onPressed: () {
                  setState(() {
                    List<Player> activePlayers =
                        players.where((element) => element.isSelected).toList();
                    if (activePlayers.length > 0) {
                      List<Player> _team1 = List<Player>();
                      List<Player> _team2 = List<Player>();
                      activePlayers.shuffle();
                      for (int i = 0; i < activePlayers.length; i++) {
                        if (i % 2 == 0) {
                          _team1.add(activePlayers[i]);
                        } else {
                          _team2.add(activePlayers[i]);
                        }
                      }
                      team1 = _team1;
                      team2 = _team2;
                    }
                  });
                },
              ),
            ),
            isLoading ? CircularProgressIndicator() : Container(),
            Padding(
              padding: EdgeInsets.all(10),
              child: RaisedButton(
                color: Colors.lime,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20)),
                child: Padding(
                  padding: const EdgeInsets.all(10.0),
                  child: Text(
                    "ScrambleAPI",
                    style: TextStyle(fontSize: 20),
                  ),
                ),
                onPressed: () {
                  PlayerApi api = PlayerApi();

                  List<Player> activePlayers =
                      players.where((element) => element.isSelected).toList();

                  setState(() {
                    isLoading = true;
                  });

                  api.fetchScrambledTeams(activePlayers).then((game) {
                    setState(() {
                      team1 = game.t.players;
                      team2 = game.ct.players;
                      isLoading = false;
                    });
                  });
                },
              ),
            ),
          ],
        ),
        Expanded(
          flex: 2,
          child: Container(
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                Expanded(
                  flex: 1,
                  child: Team(
                    imagePath: 't.png',
                    team: team1,
                    name: "Terrors",
                    color: Colors.red,
                  ),
                ),
                Expanded(
                  flex: 1,
                  child: Team(
                    imagePath: 'ct.jpg',
                    team: team2,
                    name: "CTs",
                    color: Colors.blue,
                  ),
                )
              ],
            ),
          ),
        )
      ],
    );
  }
}

class Team extends StatelessWidget {
  const Team({
    Key key,
    @required this.team,
    @required this.color,
    @required this.name,
    @required this.imagePath,
  }) : super(key: key);

  final List<Player> team;
  final Color color;
  final String name;
  final String imagePath;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          name,
          style: TextStyle(
              color: color, fontWeight: FontWeight.bold, fontSize: 30),
        ),
        Expanded(
          child: Container(
            child: ListView.builder(
              itemCount: team.length,
              itemBuilder: (BuildContext context, int index) {
                return Card(
                  child: Column(
                    children: [
                      ListTile(
                        leading: Image(
                          image: AssetImage(imagePath),
                        ),
                        title: Text(
                          team[index].name,
                          style: TextStyle(fontSize: 25),
                        ),
                        subtitle: Text(
                          team[index].steamName,
                          style: TextStyle(
                              color: color, fontWeight: FontWeight.bold),
                        ),
                      ),
                      Align(
                          alignment: Alignment.bottomRight,
                          child: Text(
                            team[index].skillScore.toString(),
                            style: TextStyle(
                                fontStyle: FontStyle.italic,
                                color: Colors.purple),
                          ))
                    ],
                  ),
                );
              },
            ),
          ),
        )
      ],
    );
  }
}
