import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:NoCrybabies/model/player.dart';
import 'package:NoCrybabies/model/skill.dart';

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

class Images extends StatelessWidget {
  final String imagePath;
  Images({@required this.imagePath});
  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 100,
      child: Row(
        children: [
          Image(
            image: AssetImage(imagePath),
          ),
          Image(
            image: AssetImage("assets/global_elite.png"),
          )
        ],
      ),
    );
  }
}

class SkillWidget extends StatelessWidget {
  final Skill skill;
  SkillWidget({@required this.skill});

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Row(
        children: [
          Text(
            skill.skillScore.toStringAsFixed(3),
            style: TextStyle(fontStyle: FontStyle.italic, color: Colors.purple),
          ),
          SizedBox(
            width: 10,
          ),
          SkillFormIcon(skillTrend: skill.skillTrend),
        ],
      ),
    );
  }
}

class SkillFormIcon extends StatelessWidget {
  final String skillTrend;

  SkillFormIcon({@required this.skillTrend});

  _getFormIcon() {
    if (skillTrend == PLATEAU) {
      return Icon(
        Icons.trending_flat,
      );
    }
    if (skillTrend == UPWARDS) {
      return Icon(
        Icons.trending_up,
        color: Colors.teal,
      );
    }
    if (skillTrend == DOWNWARDS) {
      return Icon(
        Icons.trending_down,
        color: Colors.red,
      );
    }
    return Icon(Icons.trending_flat);
  }

  @override
  Widget build(BuildContext context) {
    return _getFormIcon();
  }
}
