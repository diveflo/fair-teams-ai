import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:frontend/player.dart';

class PlayerSelection extends StatefulWidget {
  @override
  _PlayerSelectionState createState() => _PlayerSelectionState();
}

class _PlayerSelectionState extends State<PlayerSelection> {
  List<Player> players;

  @override
  void initState() {
    players = List<Player>();
    players.add(Player(name: "Flo"));
    players.add(Player(name: "Hubi"));
    players.add(Player(name: "Alex"));
    players.add(Player(name: "Sandy", steamID: "76561198011654217"));
    players.add(Player(name: "Markus"));
    players.add(Player(name: "Andi"));
    players.add(Player(name: "Martin"));
    players.add(Player(name: "Ferdy"));
    players.add(Player(name: "Niggo"));

    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: [
        Text(
          "Choose the players!",
          style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
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
                  title: Text(players[index].name),
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
        RaisedButton(
          child: Text("Scramble"),
          onPressed: () {},
        ),
      ],
    );
  }
}
