import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:frontend/player.dart';

class PlayerSelection extends StatefulWidget {
  @override
  _PlayerSelectionState createState() => _PlayerSelectionState();
}

class _PlayerSelectionState extends State<PlayerSelection> {
  List<Player> players;
  List<Player> team1;
  List<Player> team2;

  @override
  void initState() {
    players = List<Player>();
    team1 = List<Player>();
    team2 = List<Player>();
    players.add(Player(name: "Flo"));
    players.add(Player(name: "Hubi"));
    players.add(Player(name: "Alex"));
    players.add(Player(name: "Sandy", steamID: "76561198011654217"));
    players.add(Player(name: "Markus"));
    players.add(Player(name: "Andi"));
    players.add(Player(name: "Martin"));
    players.add(Player(name: "Ferdy"));
    players.add(Player(name: "Niggo"));
    players.add(Player(name: "Stefan"));

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
                      style:
                          TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
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
        Padding(
          padding: EdgeInsets.all(50),
          child: RaisedButton(
            color: Colors.lime,
            shape:
                RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
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
        Expanded(
          flex: 2,
          child: Container(
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                Expanded(
                  flex: 1,
                  child: Team(
                    team: team1,
                    name: "Terrors",
                    color: Colors.red,
                  ),
                ),
                Expanded(
                  flex: 1,
                  child: Team(
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
  }) : super(key: key);

  final List<Player> team;
  final Color color;
  final String name;

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
                return ListTile(
                  title: Align(
                    alignment: Alignment.center,
                    child: Text(
                      team[index].name,
                      style:
                          TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                    ),
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
