#include "StdAfx.h"
#include "Field.h"


Field::Field(void)
{
	for(int i=0;i<100;i++)
	{
		_field[i] = 0;
	}
}

void Field::setCell(int x, int y, int value)
{
	_field[y*10+x] = value;
}

int Field::getCell(int x, int y)
{
	return _field[y*10+x];
}

int Field::shootCell(int x, int y)
{
	return _field[y*10+x]&=4;
}

bool Field::placePlane(int x, int y, int direction)
{
	

	/*
	00 null
	01 бие
	10 толгой
	11 алдаа

	100 х
	101 ш
	110 с
	111 сөнсөн

	a b  c
	ABCD EF
	0000 00
	0100 01
	0001 01
	dddd 11
	1000 10
	0010 10
	*/

	switch(direction)
	{
	
	case 0:
		if(x+4>9||y+3>9)
			return false;
		_field[y*10 + x+2] = 2;

		_field[(y+1)*10 + x] = 1;
		_field[(y+1)*10 + x+1] = 1;
		_field[(y+1)*10 + x+2] = 1;
		_field[(y+1)*10 + x+3] = 1;
		_field[(y+1)*10 + x+4] = 1;

		_field[(y+2)*10 + x+2] = 1;

		_field[(y+3)*10 + x+1] = 1;
		_field[(y+3)*10 + x+2] = 1;
		_field[(y+3)*10 + x+3] = 1;
	


		break;

	case 1:
		if(x+3>9||y+4>9)
			return false;
		_field[(y+2)*10 + x+3] = 2;

		_field[(y+0)*10 + x+2] = 1;
		_field[(y+1)*10 + x+2] = 1;
		_field[(y+2)*10 + x+2] = 1;
		_field[(y+3)*10 + x+2] = 1;
		_field[(y+4)*10 + x+2] = 1;

		_field[(y+2)*10 + x+1] = 1;

		_field[(y+1)*10 + x+0] = 1;
		_field[(y+2)*10 + x+0] = 1;
		_field[(y+3)*10 + x+0] = 1;

		break;

	case 2:
		if(x+4>9||y+3>9)
			return false;

		_field[(y+3)*10 + x+2] = 2;

		_field[(y+2)*10 + x] = 1;
		_field[(y+2)*10 + x+1] = 1;
		_field[(y+2)*10 + x+2] = 1;
		_field[(y+2)*10 + x+3] = 1;
		_field[(y+2)*10 + x+4] = 1;

		_field[(y+1)*10 + x+2] = 1;

		_field[(y+0)*10 + x+1] = 1;
		_field[(y+0)*10 + x+2] = 1;
		_field[(y+0)*10 + x+3] = 1;

		break;

	case 3:
		if(x+3>9||y+4>9)
			return false;
		_field[(y+2)*10 + x+0] = 2;

		_field[(y+0)*10 + x+1] = 1;
		_field[(y+1)*10 + x+1] = 1;
		_field[(y+2)*10 + x+1] = 1;
		_field[(y+3)*10 + x+1] = 1;
		_field[(y+4)*10 + x+1] = 1;

		_field[(y+2)*10 + x+2] = 1;

		_field[(y+1)*10 + x+3] = 1;
		_field[(y+2)*10 + x+3] = 1;
		_field[(y+3)*10 + x+3] = 1;
		break;
	}
	return true;
}


Field::~Field(void)
{
	delete[] _field;
}
