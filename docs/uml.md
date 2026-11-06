# UML 다이어그램

> 분류: Explanation (그림으로 이해하기)

GameDataServer의 구조와 요청 흐름을 PlantUML로 그린 그림입니다.

- 그림 원본: [`docs/diagrams/`](diagrams/) 폴더의 `.puml` 파일
- VS Code에서 보기: PlantUML 확장(jebbs)을 설치하고 `.puml` 파일을 연 뒤 `Alt + D`
- GitHub에서는 아래 그림이 바로 보입니다 (plantuml.com 서버가 그림을 그려 줍니다)

| 번호 | 그림 | 종류 | 원본 |
|---|---|---|---|
| 1 | 레이어 구조 | 클래스 다이어그램 | [`architecture.puml`](diagrams/architecture.puml) |
| 2 | 데이터 모양 | 클래스 다이어그램 | [`models.puml`](diagrams/models.puml) |
| 3 | 로그인 흐름 — `POST /auth/login` | 시퀀스 다이어그램 | [`login-sequence.puml`](diagrams/login-sequence.puml) |
| 4 | 몬스터 추가 흐름 — `POST /monsters` | 시퀀스 다이어그램 | [`add-monster-sequence.puml`](diagrams/add-monster-sequence.puml) |
| 5 | 랭킹 흐름 — `POST /ranking`, `GET /ranking/top` | 시퀀스 다이어그램 | [`ranking-sequence.puml`](diagrams/ranking-sequence.puml) |
| 6 | 배포 구조 | 배포 다이어그램 | [`deployment.puml`](diagrams/deployment.puml) |

## 1. 레이어 구조 (클래스 다이어그램)

요청은 Endpoints → Services → Repositories → DataSources → DB 순서로만 흐릅니다. 점선 삼각형 화살표는 "인터페이스를 구현한다", 실선 화살표는 "이것에 기댄다"는 뜻입니다. 위 계층은 아래 계층의 **인터페이스**에만 기댑니다(의존성 역전).

![레이어 구조 (클래스 다이어그램)](https://www.plantuml.com/plantuml/svg/XLPTJnD157tVNt6Z3y839IAy66FymgYH12c9pqDtA2lRtJgp5OcIY3R4IEDN_2X9QqeaCHfDq81WmXC_XSVkx7zmPdRBpixi0Y6qU-_PE-UUEtE6ApH096ZKN42aik04k18q2BREGTXxoRexxDCEz7T_iitjYy0wJVRvBMjsW7riXb_QuMOd_DQ2gBtF_dQYZvrmVQi4qSO7zdmdUjU1SBNB2m3hxaVDxUXrMrHxjSKVeV_d1Iy4xEj1-ECx3B5kcstkiEPlO9z_iLvps4Akwo_LdCUELxNeekFL4K4re0l8zfTu3EQHIx6IgQFA8ghYSh3iOY2y1UHLNHNW-JQck0vZ4qgG86zH1CVLOCL5bC8J2-0QBrebViEph_kkJ-2-K4V4VM9Z4iVEJiWVQyLQS6mCk5OFbg66QtEOKCjAQC6PIS-k-uuNKDxirmEsreAZjVTmQtPscilpBbpV7Ju3YCBqURbmJ67AzsY0oV6hnuaPJfkBO2QkDe85Cpf9Y4zcC6suFBYYa2fZyiYfOCwflzTX_pOI3cEI0o-1oJogOBYT44dWsLp29JSdwAI9J5Dws2YJDZI8gSn578QYjLwqkfsmlg2pdlKNiQSNlBCK5CNBE0WuXQn4Dr20HcTmtQTEu1D7Q3N5TvBBTpw4RrfXRukFixy_835U8DrnXULSzOhIqovQrbxEg9YJoQiOPunYHf_YJzbl4BabolVk7kv5hHRh7YJDJHGqbxvMi6smNGH9s1QbOygPRDnB-Q7BZsBU-iaugU_TT5nSH46jaiT1pPz2GYJc49Nxqqw-ymrQhqj9gwY6I_QS551yj-SKaBBoQF9zW1NEnB7pl8UqM2tkXfOUyFOaZFB6B6xLHn-UyLz8NKQuIxB7TVzDm9PkBp0oSjauwxhLn23jj6Pi9qQe1zTOg5Iwd3KbOnK3aLt2IASIwCrUUbegYVihQZUvpSbFUIV9-qlSGVszVN6AlpNvvJKyq4QJ8QwYIQEf46CqPLGTOe0gJCfw015ZIq_euPxqgbC6E7XN7PzooZMMwlmKbCfJGWotC10P7yqynPtgLgizooAgxLec-Opk2XUpWOpwAbBl8mOQtQMZKA1Y6kcvFsKCodlg91JVE1ceUPJPeJev1MWqg8nFWLaceLYA8YlDvvMCzaHxDKcgAvaT66HLkDb8UjJNjrYl0_4TDPX6GJjI5SsmynSeHffj5K87ZnAH-7vE9d7v4-MKGEdpLx1dyt_JhVy0)

원본: [`diagrams/architecture.puml`](diagrams/architecture.puml)

## 2. 데이터 모양 (클래스 다이어그램)

바깥에서 온 데이터는 DTO(값이 빠질 수 있음), 서버 안에서는 Model(record, null 없음)을 쓰고 Mapper가 둘을 바꿉니다. 실패는 예외 대신 `Result`의 `Failure(AppError)`로 돌려줍니다.

![데이터 모양 (클래스 다이어그램)](https://www.plantuml.com/plantuml/svg/XLLDInn14BtFhoWsX-mIzPQBo4RZ1mhHY2vuRcVQTN2sU-ZkZP54aB2890P2C6X8348-3nuMJI12Vj7E-1zIqpspqwCpk0VPVbNxNdMzgdPIAY9KjnD0XtiqaCvz65psuhz_87vr7f_-Ze_VnRtF47_zDxWyY_gVejT74BtjO-Iwrn-7g7yqkFeUd-x1JEjfdNM3WAm7j342yOU315rCIEk2kbnuY4QNUzSVJtI0X24LZjpoMKW4wO3S91xVzbaRDaWWgHLn0o8bl7007XDtgorubtdJFE02rZPzHHCSsQamsBs7-kFiEaw8wQHDeJQZk8IwgJMlVWG6_VVu5QAhi_Zd0SG79n1_mR8F6pKjPuGNEPEA2kJG8C231G_6mMVgaJakaGv5G2g1rQVOV5X8cL8AQob0go6b2Sz6m8d6TeUAAuJXvTit5Gi2gzXHcXFcFruEo4wXshBobZ5p6visJ-Hc8PRpFE5jdraqXhUAg8AanRSeMw4on1vIc9WmWz1iPgHJhakbr5b31XFHM6idf3VmsUUXBwXSOERkMissMWyScer36rtqaZ6q3Ky65EBZ_GgFIohCl2xfPT7fsqQN-5ncyonJOgTAsLQoB2znl8IXfDfIusytKI-dBEcGYQ0MFXlATtEj5ZUhdczB0_dJKvQ2Q9PWHH4jwykTKlcQ96HsDsqz6xVoRegQ9jwfETokXCeSR363vB5CnMxkTE9YFNxpxVhm5yGN9z6FytGMoJhwGLoLbbJ3nUa6QgBLh071jkYJJipBhgrszGhLMp1356dep1JBa-Q87tG5hK-5uQmG-4ZgloOvZM4oPTqE35FCnPUucalUMtrOOCz8u7l9MDbd_L9eO4rmriv6BBjuSNB6nfgsxvL1fpW5ISHohZ9ex2-OcKHpDof2ZfCs3KP7NpRJNZjfRogmvh1PpYHb7luFTVu3)

원본: [`diagrams/models.puml`](diagrams/models.puml)

## 3. 로그인 흐름 — `POST /auth/login` (시퀀스 다이어그램)

처음 보는 이름이면 가입, 있는 이름이면 비밀번호를 확인하고, 통과하면 30분짜리 JWT를 발급합니다.

![로그인 흐름 — `POST /auth/login` (시퀀스 다이어그램)](https://www.plantuml.com/plantuml/svg/bLNDJnD15B_lN_7IYst2DnmC1mA5Oa023ItXGcA6xjXksCuiExDWDIGrGOBYWGCWHZ2O25vCR02r9iG_ncDt-p_uPhOLbfOFBxixyzxlzxxV3Wj9NEaLRR1vtcATWYvxbEMeyG3ICvaiT1DF5hgr3FwKTy3_T53xMGqEg_NT_ThP4TGtZkelA-1NFjIg5KCiMSmXBYc2A12Jhrei3q-9BQY1D9nvnKNg6WR9IUv2R8vPiWJTaEQhrCqKg6t7W0WOjItAf84qqifP3c4IHj2753CTRZ4fMYGPwgvOwF7L-xHDIjITfGuN5XehHOIPPTkID5GP8v9akESYtiJFHI8enFAaIBlCnGKMTxIII6ZFnf8HcibLcULBb3Nj6w7dq3aKTHW6MrAvm5v0BEIE3K9ioIh6El222B7ANLDTzVRr3yHWpOWoDQWR5f5uIh6DY1BBnVNdcEG9JB0j8MG7Vt_RVxC3JUfQfGp-h_LWxno2Zvl1uLi38eoTRPmV9vRjkJG-mLQ8RPdQJgA1k-8Q8Xi968I1dXv84XDcLJS9QL1i0JJuoZyw3WvlCZtKKZLaUaHbiZIDzGY3rAbInbjK4Tw-hhUGt010gh4amZAfgTHe5XW2E_vby4c1Y09qTNKX7hKwRxDRttlTB43mRYCC-IQRxSDlw4dUv66URRSDlwKrCbukHuM8Xwg99a6pcjDSZdEFcTWk0AfXWjDzz13yitFjx-6vVx8ELzG0MkeLDOXzvLVA_kbcVR_g7rz0VVSys3h0XO7ROQVs-yBV-joEf4tK8wOPLghXkuRzLyKdcA2kr2GQV4Ub9wOpgTailh8pZS4H20kx5XFnhQfQzZ8jT-SYZ3ruNuvaHHrgtxSrd3io8bEJMTyg-oSNMdp_iPntESkd6zsPk0A-SJPxOOwfNSHTwparDO8ozLRFrgcyjkUGOjIbHDBBERgkqTgCMZnBXSEnC13lx_5_h0TV3loJhp0vdqtSOmirspd2TCSQwiCrDFDOBrkYqHgirYlEhzguKfsp9OUgkoGbBkvZlARF7CkbOaAfzZv4aZL3fMOO7_YdDFu2)

원본: [`diagrams/login-sequence.puml`](diagrams/login-sequence.puml)

## 4. 몬스터 추가 흐름 — `POST /monsters` (시퀀스 다이어그램)

토큰 검사는 ASP.NET Core가 Endpoints보다 먼저 합니다. 그래서 토큰이 없으면 서버 코드까지 오지도 않고 401이 나갑니다.

![몬스터 추가 흐름 — `POST /monsters` (시퀀스 다이어그램)](https://www.plantuml.com/plantuml/svg/bLH1JnD15BxlhtZPYssafHgzDDhG0YOSHCAMMo_JxaWtR6VBpYm4ZKcHIb1wC06KQDjWg6W8nWPGQjAJFyLZT_eVVDjkeT12zB0xavdvlldU-zwRCIw8BPoS2KJNGpcBSK7j4AUB3cKPgjo2cITQ4aRz3Gv_2jlGNjzhhzPbvHHaTKE-EvKxJN0FZ-IRMhjOTpytGVxSRjKBxPqza1kxtY5tiwRm1OFbYKromBD4jvODDWzFYScfGXnXCIUNfhQYa8omR53dc25MO1Hch6LgQrbgcYeG3kEcGPbGa4OO6IDFc00rhis4foUJC6xPDCLafI4FAk1-RxXRDVdnAyhh8EEEo5x2FUv6DCdql6KmmOTjQjHUCZ0BGxPcQTxY1cfTkRIhBPg6eFwP2IA8PZasCkWuJHDEGPqdEHhMqoaMwEKqs14uaL2KRdWGYdNaGlHAybFCMxPiunaHXiMYaA34fZOyw1eIIx7de3BaLwEWkbjbjxhREgcf8w1cyxXqFn8PWN0u32-K3lt5DR9OTWzV_Jvpw-LMuxMiliLfgI3NLd7yKdBtIz0wBiYNHsYFAVhjVxykAoMqgRrPS0-QEEaULw3B7C8h_9YYS2zo1-OOyIEWkaBHUvyCXn-ja-OvBZPWZwVomxOy_WQtUnYvtqIt0zn9mqDG5uoS6aI20U05cMydKiLrFSvNM2RGSqfOGGN1NdWOc7jGzC9pVnMnV3f5Nl5YWgj4eU527n73T6mQc69Bn3JqQNGas0_lqtOvGH5841rclURZ0X6T35tekK58RB0kXuHvIKNVIOHVKxX8CiKujSLWkZogwr18E97miDFQv6mIX-GJE7z2i0GxO3mHkl5ckRO1XdxpHTT4tWST4kwmal0N8D251l_3Ryt9P2ZdGxCyh0xxiUwdSkkiWUtQUp_znnDtUWsCalptj0pOX6wr50Mizg3w3pLr5vjktAP4K2yPb67NuJU6FtpobRy0)

원본: [`diagrams/add-monster-sequence.puml`](diagrams/add-monster-sequence.puml)

## 5. 랭킹 흐름 — `POST /ranking`, `GET /ranking/top` (시퀀스 다이어그램)

같은 이름으로 다시 저장하면 SQL의 `MAX`로 더 높은 점수만 남깁니다.

![랭킹 흐름 — `POST /ranking`, `GET /ranking/top` (시퀀스 다이어그램)](https://www.plantuml.com/plantuml/svg/ZLLFQnD17BtFf_Yn5nD8qaGy5LBQPBSbs2OXkmN5UfZkZdRePdQxCziIH8XOeK8FWb94RB6W1tigw4NmqtXijj_1NpQh-RjfJjdDvBrvl_VUJ5Qbee4Asou4LEnpyJmlsK78XCt8FMWsJ0kMaeKSR1Z3joNb-V2d-m6YB--Yau_uqOqkljty-0tHwrVHvsCe5NjNRo2wlBxzT4eaGdmQq3R8FUfuHqW0pwWh6Q6XyaJOtcK18THMNW3Qjk2g0qlGz8vOOEunrzM0IgYwd0b5a4Pnc_jKA6WDj1Z2yJqkb9orQBBWaECqCvPQpFSanpqxOwlcWSiLItwZKqLDBmoGmS77NIePQCbQmTdT4Pd4a6miKgyGKYwFcmBbCXc8X_pAb6PO7hTvHxm0ZJlQCcZxlAtbG9Es5p1yBzql5k4bcI8OaYQp8gL93zcQx0WxysyEvMNHOLS1Tswkkz3xTHoT_OJeuYGwFoKmoP2VhNITSZSCMAOc3gdBdJfji-mGF289uSd8o_00PLUe0wr-iQGY37DFLAHklpATKJ9L1IdClYDZunb21PqHAICmv4f951ctVSa2DPipjWnImSYhLv2YLZUDbWM5GW4QTQWswkkRjQgLuKvsH-WDs6xgQvO19fwTE4WemzRQeqpydCF3SfAK9Rjm26PesqpAjD5JKkYNf_6GCE4C6ZfzIk_iwUJvNwoA6qnPdZzmDaiM2teKKoh-HosQwYJyhWXDOzEeMjZ9N99H76PBDrfGUPnyfHjc5JPhMpKBJIC8oy-LS7lsjlV-Q_hEAOCFgmUzg--zo_DPeyzoURGC4z0xwZG7ERTAyKMLj05lfoUnQK_nWbh5Zk4V2laB)

원본: [`diagrams/ranking-sequence.puml`](diagrams/ranking-sequence.puml)

## 6. 배포 구조 (배포 다이어그램)

Unity 게임은 Render에 올라간 서버와 통신합니다. 내 PC에서 고친 코드는 GitHub에 push해야 Render에 반영됩니다.

![배포 구조 (배포 다이어그램)](https://www.plantuml.com/plantuml/svg/NLFFInj15BxFN_78fPRMc8kN78fHGIdz8MflkKooOx9rCxFTcRIaHOXb3M8NB6sKMDmWj20j5fQmXGZ-DJqwi_z3PsPNY3tiCFlc-xxtlVTcbhZ0WUXsF728xx5UXr217e0y36-JFgZJL0w7AZm7_Sb9gCxwa0t-g4yNsSb87WtLPRyAhwWhUf3jfpA-KN6gJgVPqHJ-3hx09g4E2LGyWZNS8QjOu2qIl2M1FLnTXYoAL7o3-AvBVHpW3l0sTjWxbxPW1tkS88IRWWMG3IDvSMYrKqIPGw2K9xsTH6eSrkdSSqQv8C6Avsh_DTwZpOSbm3ptLb3MNB7URTHf1nieo6aan-VgUcI1-X2XfiUw3fHosp2tofgx9D127n10eN6lZZfj2-7pwi92IyST7USsFlzcqMsL6GsiKBd9EZP7KRnH0p3eXiP3gURxHj9gb1rZC2-6FmRVmpszoHsQr8t_k9kOwkQrBB_OMuaWtrkYYHV47TURINYNf_oQCrgdQZ9ISGGo_AxY4EIlAtNq8mkJmZfnjC8UsaD5D-N753PMwjHZJUor6HVLnKgbOi7FJ0YXsQd1_FoJOXZvvCn_qOygh6zlRy0ZUBhryeMsyJMvdIQWlWtLvBUA-r3hYZOBtFTOk8nMOPdWGBEomKMsdo0zj_jQVfUtpUNycO8QVvR7Po2J93k-GWN2W8jMpaIA7faoeLo-GsYvx41VqLHEXte5UHsQMjJuq7Re99AN4RBLvYJjfGejlHeJQ4bFNpyiz0y0)

원본: [`diagrams/deployment.puml`](diagrams/deployment.puml)

## 그림을 고쳤을 때

1. `docs/diagrams/`의 `.puml` 파일을 고치고 `Alt + D`로 미리 봅니다.
2. 위의 그림 주소(`https://www.plantuml.com/plantuml/svg/...`)는 원본 내용을 압축한 값이라, 원본을 고치면 주소도 바뀌어야 합니다.
   VS Code에서 `Ctrl + Shift + P` → `PlantUML: Export Current Diagram`으로 `svg`를 저장해 `docs/diagrams/`에 넣고, 그림 주소를 `diagrams/파일이름.svg`로 바꿔도 됩니다.
