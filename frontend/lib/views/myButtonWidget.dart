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
    return RaisedButton(
      child: Text(
        buttonText,
      ),
      onPressed: isDisabled ? null : onPressed,
      color: color,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
    );
  }
}
