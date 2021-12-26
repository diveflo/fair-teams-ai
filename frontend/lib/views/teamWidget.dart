import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/views/skillWidget.dart';

class TeamWidget extends StatelessWidget {
  const TeamWidget({
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
        Container(
          margin: EdgeInsets.only(bottom: 5),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Image(
                image: AssetImage(imagePath),
                height: 40,
              ),
              SizedBox(
                width: 10,
              ),
              Text(
                name,
                style: TextStyle(
                    color: color, fontWeight: FontWeight.bold, fontSize: 30),
              ),
            ],
          ),
        ),
        Expanded(
          child: Container(
            child: ListView.builder(
              itemCount: team.length,
              itemBuilder: (BuildContext context, int index) {
                return Card(
                  shape: RoundedRectangleBorder(
                      side: BorderSide(color: color, width: 2),
                      borderRadius: BorderRadius.circular(8)),
                  child: Column(
                    children: [
                      ListTile(
                        leading: Image(
                          image: AssetImage(
                              "assets/" + team[index].skill.rank + ".png"),
                        ),
                        title: Text(
                          team[index].name,
                          style: TextStyle(fontSize: 20),
                        ),
                        subtitle: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(team[index].steamName,
                                style: TextStyle(
                                    color: color, fontWeight: FontWeight.bold)),
                            SkillWidget(skill: team[index].skill),
                          ],
                        ),
                      ),
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
