import 'package:flutter/material.dart';

class ImageWidget extends StatelessWidget {
  final String imagePath;
  ImageWidget({@required this.imagePath});
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
