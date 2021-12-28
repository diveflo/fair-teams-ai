import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/skill.dart';

class SkillFormIconWidget extends StatelessWidget {
  final String skillTrend;

  SkillFormIconWidget({@required this.skillTrend});

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
