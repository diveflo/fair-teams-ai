import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/skill.dart';
import 'package:no_cry_babies/views/fractions/teams/playerCard/skillFormIconWidget.dart';

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
          SkillFormIconWidget(skillTrend: skill.skillTrend),
        ],
      ),
    );
  }
}
