import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:frontend/model/player.dart';
import 'package:frontend/model/skill.dart';

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
          child: Text(
            name,
            style: TextStyle(
                color: color, fontWeight: FontWeight.bold, fontSize: 30),
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
                          image: AssetImage(imagePath),
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
          SkillFormIcon(form: skill.skillTrend),
        ],
      ),
    );
  }
}

class SkillFormIcon extends StatelessWidget {
  final int form;

  SkillFormIcon({@required this.form});

  _getFormIcon() {
    if (form == 0) {
      return Icon(
        Icons.trending_flat,
      );
    }
    if (form == 1) {
      return Icon(
        Icons.trending_up,
        color: Colors.teal,
      );
    }
    if (form == 2) {
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
