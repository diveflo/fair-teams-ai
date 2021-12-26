import 'package:flutter/material.dart';

class MyButtonWidget extends StatelessWidget {
  const MyButtonWidget({
    Key key,
    @required this.onPressed,
    @required this.color,
    @required this.buttonText,
    this.isDisabled = false,
  }) : super(key: key);

  final VoidCallback onPressed;
  final Color color;
  final String buttonText;
  final bool isDisabled;

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
        onPressed: isDisabled ? null : onPressed,
        child: Text(
          buttonText,
        ),
        style: ElevatedButton.styleFrom(
            primary: color,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20),
            )));
  }
}
