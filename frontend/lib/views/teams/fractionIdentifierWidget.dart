import 'package:flutter/material.dart';

class FractionIdentifierWidget extends StatelessWidget {
  const FractionIdentifierWidget({
    Key key,
    @required this.imagePath,
    @required this.name,
    @required this.color,
  }) : super(key: key);

  final String imagePath;
  final String name;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Container(
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
    );
  }
}
