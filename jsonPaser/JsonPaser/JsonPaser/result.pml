int s_level = 0;
bool s_alarm = false;
int t_level = -10;
bool t_alarm = false;
mtype s_detector_state;
mtype t_detector_state;
mtype fan_state;
mtype fan_open_auth;
chan fan_open = [0] of {byte}
mtype fan_close_auth;
chan fan_close = [0] of {byte}
mtype={s_detector_S1,s_detector_S2,t_detector_S1,t_detector_S2,fan_Closed,fan_Open,fan_open_env,fan_open_trigger,fan_close_env,fan_close_trigger};

active proctype s_detector()
{
beginning:
S1:
atomic{
s_detector_state=s_detector_S1;
do
::s_level<30->s_level++
::break
od
goto S2;
}
S2:
atomic{
s_detector_state=s_detector_S2;
if
::s_level<=20->atomic{
s_alarm=false;
goto S1;
}
::s_level>20->atomic{
s_alarm=true;
goto S1;
}
fi
}
}

active proctype t_detector()
{
beginning:
S1:
atomic{
t_detector_state=t_detector_S1;
do
::t_level<30->t_level++
::break
od
goto S2;
}
S2:
atomic{
t_detector_state=t_detector_S2;
if
::t_level>=18->atomic{
t_alarm=false;
goto S1;
}
::t_level<18->atomic{
t_alarm=true;
goto S1;
}
fi
}
}

active proctype fan()
{
beginning:
Closed:
atomic{
fan_state=fan_Closed;
fan_open?1->atomic{
goto Open;
}
}
Open:
atomic{
fan_state=fan_Open;
fan_close?1->atomic{
goto Closed;
}
}
}

active proctype s_detector_trigger1()
{
beginning:
S1:
atomic{
s_alarm==true->atomic{
goto S2;
}
}
S2:
atomic{
fan_open_auth=fan_open_trigger
fan_open!1;
goto S1;
}
}

active proctype t_detector_trigger1()
{
beginning:
S1:
atomic{
t_alarm==true->atomic{
goto S2;
}
}
S2:
atomic{
fan_close_auth=fan_close_trigger
fan_close!1;
goto S1;
}
}

